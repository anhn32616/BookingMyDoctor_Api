using booking_my_doctor.Services.PaymentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace StarCinema_Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    

    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [Authorize(Roles = "ROLE_ADMIN, ROLE_DOCTOR")]
        [HttpGet]
        public async Task<IActionResult> GetPaymentListAsync(int? page = null, int? pageSize = null, int? doctorId = null, bool? status = null, DateTime? date = null)
        {
            var role = User.FindFirstValue("Role");
            if(role == "ROLE_DOCTOR")
            {
                doctorId = Convert.ToInt32(User.FindFirstValue("DoctorId"));
            }
            var resData = await _paymentService.GetPaymentListAsync(page, pageSize, doctorId, status, date);
            return StatusCode(resData.statusCode, resData);
        }

 
        [Authorize(Roles = "ROLE_ADMIN, ROLE_DOCTOR")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymenByIdAsync(int id)
        {
            var resData = await _paymentService.GetPaymentById(id);
            return StatusCode(resData.statusCode, resData);
        }

        [Authorize(Roles = "ROLE_ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var resData = await _paymentService.DeletePayment(id);
            return StatusCode(resData.statusCode, resData);
        }
        [Authorize(Roles = "ROLE_DOCTOR")]
        [HttpPost()]
        public async Task<IActionResult> CreatePayment()
        {
            var doctorId = Convert.ToInt32(User.FindFirstValue("DoctorId"));
            var resData = await _paymentService.CreatePayment(doctorId);
            return StatusCode(resData.statusCode, resData);
        }
        [Authorize(Roles = "ROLE_DOCTOR")]
        [HttpGet("info")]
        public async Task<IActionResult> GetPaymentInfo()
        {
            var doctorId = Convert.ToInt32(User.FindFirstValue("DoctorId"));
            var resData = await _paymentService.GetPaymentInfo(doctorId);
            return StatusCode(resData.statusCode, resData);
        }
        [Authorize(Roles = "ROLE_DOCTOR")]
        [HttpGet("return")]
        public async Task<IActionResult> ReturnPayment()
        {
            var vnpayData = Request.Query;
            var resData = await _paymentService.ReturnPayment(vnpayData);
            return StatusCode(resData.statusCode, resData);
        }
    }
}
