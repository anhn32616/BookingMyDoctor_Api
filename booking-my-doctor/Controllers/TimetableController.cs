using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs.Timetable;
using booking_my_doctor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace booking_my_doctor.Controllers
{
    [Route("api/timetable")]
    [ApiController]
    [Authorize(Roles = "ROLE_DOCTOR")]
    public class timetableController : Controller
    {
        private readonly ITimetableService _timetableService;
        public timetableController(ITimetableService timetableService)
        {
            _timetableService = timetableService;
        }
        [HttpGet]
        public async Task<IActionResult> Gettimetables(int? page = 0, int? pageSize = int.MaxValue, int? doctorId = null) 
        {
            var resData = await _timetableService.GetTimetables(page, pageSize, doctorId);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GettimetableById(int id)
        {
            var resData = await _timetableService.GetTimetableById(id);
            return StatusCode(resData.statusCode, resData);
        }

        [HttpPost]
        public async Task<IActionResult> Createtimetable([FromBody] TimetableCreateDto timetableCreateDto)
        {
            var doctorId = Convert.ToInt32(User.FindFirstValue("DoctorId"));
            var resData = await _timetableService.CreateTimetable(timetableCreateDto, doctorId);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Updatetimetable(int id, [FromBody] Timetable timetable)
        {
            var resData = await _timetableService.UpdateTimetable(id, timetable);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletetimetable(int id)
        {
            var resData = await _timetableService.DeleteTimetable(id);
            return StatusCode(resData.statusCode, resData);
        }
    }
}
