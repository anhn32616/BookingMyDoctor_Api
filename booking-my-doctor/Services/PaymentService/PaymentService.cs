using AutoMapper;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using booking_my_doctor.DTOs.Payment;
using booking_my_doctor.Repositories;
using booking_my_doctor.Services.VnPayService;

namespace booking_my_doctor.Services.PaymentService
{

    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IVnPayService _vnPayService;
        private readonly IMapper _mapper;

        public PaymentService(IPaymentRepository paymentRepository, IVnPayService vnPayService, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _vnPayService = vnPayService;
            _mapper = mapper;
        }

        public async Task<ApiResponse> CreatePaymentAsync(Payment payment)
        {
            try
            {
                await _paymentRepository.CreatePaymentAsync(payment);
                await _paymentRepository.IsSaveChange();
                var paymentLast = await _paymentRepository.GetLastPayment();
                var res = await _vnPayService.CreateUrlPayment(paymentLast.Id, paymentLast.TotalFee);
                return res;
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }
        }


        public async Task<ApiResponse> DeletePayment(int id)
        {
            try
            {
                var payment = await _paymentRepository.GetPaymentById(id);
                if (payment == null) return new ApiResponse()
                {
                    statusCode = 404,
                    message = "Payment with this id does not exist"
                };
                _paymentRepository.DeletePayment(payment);
                await _paymentRepository.IsSaveChange();
                return new ApiResponse()
                {
                    statusCode = 200,
                    message = "Thành công"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }
        }


        public async Task<ApiResponse> GetPaymentListAsync(int? page = null, int? pageSize = null, int? doctorId = null, bool? status = null, DateTime? date = null)
        {
            try
            {
                var pagination = await _paymentRepository.GetPaymentListAsync(page, pageSize, doctorId, status, date);
                var paginationDto = _mapper.Map<PaginationDTO<Payment>, PaginationDTO<PaymentDto>>(pagination);
                return new ApiResponse()
                {
                    statusCode = 200,
                    message = "Thành công",
                    data = paginationDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }
        }


        public async Task<ApiResponse> GetPaymentById(int id)
        {
            try
            {
                var payment = await _paymentRepository.GetPaymentById(id);
                if (payment == null) return new ApiResponse()
                {
                    statusCode = 404,
                    message = "Payment with this id does not exist"
                };
                return new ApiResponse()
                {
                    statusCode = 200,
                    message = "Thành công",
                    data = payment
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }

        }

        public async Task<ApiResponse> CreatePayment(int doctorId)
        {
            try
            {
                await _paymentRepository.CreatePayment(doctorId);
                await _paymentRepository.IsSaveChange();
                var paymentLast = await _paymentRepository.GetLastPayment();
                var res = await _vnPayService.CreateUrlPayment(paymentLast.Id, paymentLast.TotalFee);
                return res;
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }
        }

        public async Task<ApiResponse> GetPaymentInfo(int doctorId)
        {
            try
            {
                var res = await _paymentRepository.GetPaymentInfo(doctorId);
                return new ApiResponse()
                {
                    statusCode = 200,
                    message = "Thành công",
                    data = res
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }
        }

        public async Task<ApiResponse> ReturnPayment(IQueryCollection vnpayData)
        {
            var res = await _vnPayService.ReturnPayment(vnpayData);
            return res;
        }
    }
}
