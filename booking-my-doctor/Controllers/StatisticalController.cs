using booking_my_doctor.DTOs;
using booking_my_doctor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace booking_my_doctor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticalController : ControllerBase
    {
        private readonly IStatisticalService _statisticalService;
        public StatisticalController(IStatisticalService statisticalService)
        {
            _statisticalService = statisticalService;
        }
        [HttpGet]
        public async Task<IActionResult> GetStatistical(DateTime startTime, DateTime endTime, int? page = null, int? pageSize = null, int? doctorId = null)
        {
            var resData = await _statisticalService.GetStatistical(startTime, endTime, page, pageSize, doctorId);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpGet("quantity")]
        public async Task<IActionResult> GetQuantityStatistics()
        {
            var resData = await _statisticalService.GetQuantityStatistics();
            return StatusCode(resData.statusCode, resData);
        }
        [Authorize]
        [HttpGet("doctor")]
        public async Task<IActionResult> GetStatisticsOfDoctor(DateTime startTime, DateTime endTime)
        {
            var doctorId = Convert.ToInt32(User.FindFirstValue("doctorId"));
            var resData = await _statisticalService.GetStatisticsOfDoctor(doctorId, startTime, endTime);
            return StatusCode(resData.statusCode, resData);
        }
    }
}
