using AutoMapper;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using booking_my_doctor.DTOs.Rate;
using booking_my_doctor.Repositories;

namespace booking_my_doctor.Services
{
    public class RateService : IRateService
    {
        private readonly IMapper _mapper;
        private readonly IRateRepository _rateRepository;
        public RateService(IRateRepository rateRepository,
            IMapper mapper)
        {
            _rateRepository = rateRepository;
            _mapper = mapper;

        }

        public async Task<ApiResponse> GetRateByAppointmentId(int appointmentId)
        {
            try
            {
                var res = await _rateRepository.GetRateByAppointmentId(appointmentId);
                if (res != null)
                {
                    var resDto = _mapper.Map<Rate, RateDto>(res);
                    return new ApiResponse
                    {
                        statusCode = 200,
                        message = "Thành công",
                        data = resDto
                    };
                }
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Thành công",
                    data = null
                };

            }
            catch (Exception e)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = e.Message
                };
            }
        }

        public async Task<ApiResponse> GetRates(int? doctorId = null)
        {
            try
            {
                var res = await _rateRepository.GetRates(doctorId);
                var resDto = res.Select(_mapper.Map<Rate, RateView>).ToList();
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Thành công",
                    data = resDto
                };
            }
            catch (Exception e)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = e.Message
                };
            }
        }
    }
}
