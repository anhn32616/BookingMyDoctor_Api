using booking_my_doctor.Services;
using Microsoft.AspNetCore.Mvc;

namespace booking_my_doctor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RateController : ControllerBase
    {
        private readonly IRateService _rateService;
        public RateController(IRateService rateService)
        {
            _rateService = rateService;
        }
        [HttpGet()]
        public async Task<IActionResult> GetRates(int? doctorId = null) 
        {
            var resData = await _rateService.GetRates(doctorId);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpGet("appointment/{id}")]
        public async Task<IActionResult> GetRateByAppointmentId(int id)
        {
            var resData = await _rateService.GetRateByAppointmentId(id);
            return StatusCode(resData.statusCode, resData);
        }
    }
}
