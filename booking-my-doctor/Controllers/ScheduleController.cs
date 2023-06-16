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
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _ScheduleService;
        public ScheduleController(IScheduleService ScheduleService)
        {
            _ScheduleService = ScheduleService;
        }
        [HttpGet]
        public async Task<IActionResult> GetSchedules(int? page = 0, int? pageSize = int.MaxValue, int? doctorId = null, DateTime? date = null, string? status = null, string? sortColumn = "StartTime")
        {
            var resData = await _ScheduleService.GetSchedules(page, pageSize, doctorId, status, date, sortColumn);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetScheduleById(int id)
        {
            var resData = await _ScheduleService.GetScheduleById(id);
            return StatusCode(resData.statusCode, resData);
        }

        [Authorize(Roles = "ROLE_ADMIN, ROLE_DOCTOR")]
        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] ScheduleCreateDto scheduleCreateDto)
        {
            var role = User.FindFirstValue("Role");
            if (role == "ROLE_DOCTOR") scheduleCreateDto.DoctorId = Convert.ToInt32(User.FindFirstValue("DoctorId"));
            var resData = await _ScheduleService.CreateSchedule(scheduleCreateDto);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] ScheduleDto ScheduleDto)
        {
            ScheduleDto.Id = id;
            var resData = await _ScheduleService.UpdateSchedule(id, ScheduleDto);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var resData = await _ScheduleService.DeleteSchedule(id);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> ChangeStatusScheduleToTrue(int id)
        {
            var resData = await _ScheduleService.ChangeStatusScheduleToTrue(id);
            return StatusCode(resData.statusCode, resData);
        }
    }
}
