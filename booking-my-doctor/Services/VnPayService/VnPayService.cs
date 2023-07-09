using AutoMapper;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using booking_my_doctor.DTOs.Payment;
using booking_my_doctor.Repositories;

namespace booking_my_doctor.Services.VnPayService
{

    public class VnPayService : IVnPayService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public VnPayService(IPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse> CreateUrlPayment(int paymentId, double totalPrice)
        {

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = configBuilder.Build();

            var urlPayment = "";
            // Get Config Info
            string vnp_Returnurl = configuration.GetSection("VnPay:vnp_ReturnUrl").Value; //URL nhan ket qua tra ve 
            string vnp_Url = configuration.GetSection("VnPay:vnp_Url").Value; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = configuration.GetSection("VnPay:vnp_TmnCode").Value; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = configuration.GetSection("VnPay:vnp_HashSecret").Value;//Secret Key

            // Build URL for VNPAY

            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (totalPrice * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            vnpay.AddRequestData("vnp_BankCode", "VNBANK");


            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");

            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", paymentId.ToString());
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_IpAddr", "192.168.0.103"); 

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
            vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(5).ToString("yyyyMMddHHmmss"));




            // Add Params of 2.1.0 Version
            // Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            return new ApiResponse
            {
                statusCode = 200,
                message = "Success",
                data = urlPayment
            };

        }

        // Return info payment 
        public async Task<ApiResponse> ReturnPayment(IQueryCollection vnpayData)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = configBuilder.Build();

            string vnp_HashSecret = configuration.GetSection("VnPay:vnp_HashSecret").Value;//Secret Key
            VnPayLibrary vnpay = new VnPayLibrary();

            foreach (var kvp in vnpayData)
            {
                //get all querystring data
                if (!string.IsNullOrEmpty(kvp.Key) && kvp.Key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(kvp.Key, kvp.Value);
                }
            }
            //vnp_TxnRef: Ma don hang merchant gui VNPAY tai command=pay    
            //vnp_TransactionNo: Ma GD tai he thong VNPAY
            //vnp_ResponseCode:Response code from VNPAY: 00: Thanh cong, Khac 00: Xem tai lieu
            //vnp_SecureHash: HmacSHA512 cua du lieu tra ve



            long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));


            // Get data from payment 

            string vnpayTranId = vnpay.GetResponseData("vnp_TransactionNo");
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
            String vnp_SecureHash = vnpayData.Where(kvp => kvp.Key == "vnp_SecureHash").FirstOrDefault().Value;
            String TerminalID = vnpayData.Where(kvp => kvp.Key == "vnp_TmnCode").FirstOrDefault().Value;
            int paymentId = Convert.ToInt32(vnpayData.Where(kvp => kvp.Key == "vnp_OrderInfo").FirstOrDefault().Value);

            long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
            String bankCode = vnpayData.Where(kvp => kvp.Key == "vnp_BankCode").FirstOrDefault().Value;

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
            if (checkSignature)
            {
                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                {
                    // Payment success
                    await _paymentRepository.HandlePaymentSuccess(paymentId, vnpayTranId);
                    var payment = await _paymentRepository.GetPaymentById(paymentId);
                    var paymentDto = _mapper.Map<Payment, PaymentDto>(payment);
                    return new ApiResponse
                    {
                        statusCode = 200,
                        message = "Thành công",
                        data = paymentDto
                    };
                }
                else
                {
                    return new ApiResponse
                    {
                        statusCode = 500,
                        message = "Thanh toán thất bại",
                    };
                }
            }
            else
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = "Thanh toán thất bại",
                };
            }
        }
        
    }

}
