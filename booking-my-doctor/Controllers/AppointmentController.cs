using booking_my_doctor.DTOs;
using booking_my_doctor.DTOs.Appointment;
using booking_my_doctor.DTOs.Rate;
using booking_my_doctor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace booking_my_doctor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAppointments(int? page = null, int? pageSize = null, int? scheduleId = null, DateTime? date = null, string? status = null, int? patientId = null, int? doctorId = null, string? sortBy = "Date", bool? hiddenCancel = false)
        {
            var role = User.FindFirstValue("Role");
            var res = new ApiResponse();
            if (role == "ROLE_ADMIN")
            {
                res = await _appointmentService.GetAppointments(page, pageSize, scheduleId, date, status, patientId, doctorId, sortBy, hiddenCancel);
            } else if (role == "ROLE_PATIENT")
            {
                patientId = Convert.ToInt32(User.FindFirstValue("UserId"));
                res = await _appointmentService.GetAppointments(page, pageSize, scheduleId, date, status, patientId, doctorId, sortBy, hiddenCancel);
            } else if (role == "ROLE_DOCTOR")
            {
                doctorId = Convert.ToInt32(User.FindFirstValue("DoctorId"));
                res = await _appointmentService.GetAppointments(page, pageSize, scheduleId, date, status, patientId, doctorId, sortBy, hiddenCancel);
            }


            return StatusCode(res.statusCode, res);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var res = await _appointmentService.GetAppointmentById(id);
            return StatusCode(res.statusCode, res);
        }
        [Authorize(Roles = "ROLE_ADMIN")]
        [HttpPost("admin-create")]
        public async Task<IActionResult> AdminCreateAppointment([FromBody] AppointmentCreate appointmentCreate)
        {
            var res = await _appointmentService.CreateAppointment(appointmentCreate);
            return StatusCode(res.statusCode, res);
        }

        [Authorize(Roles = "ROLE_PATIENT")]
        [HttpPost("patient-create")]
        public async Task<IActionResult> PatientCreateAppointment([FromBody] AppointmentCreate appointmentCreate)
        {
            var patientId = User.FindFirstValue("UserId");
            appointmentCreate.PatientId = Convert.ToInt32(patientId);
            var res = await _appointmentService.CreateAppointment(appointmentCreate);
            return StatusCode(res.statusCode, res);
        }
        [Authorize(Roles = "ROLE_PATIENT, ROLE_DOCTOR")]
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var role = User.FindFirstValue("Role");
            var userId = Convert.ToInt32(User.FindFirstValue("UserId"));
            var res = await _appointmentService.CancelAppointment(id, userId, role);
            return StatusCode(res.statusCode, res);
        }

        [Authorize(Roles = "ROLE_DOCTOR")]
        [HttpPut("doctor-accept/{id}")]
        public async Task<IActionResult> DoctorAcceptAppointment(int id)
        {
            var doctorId = Convert.ToInt32(User.FindFirstValue("DoctorId"));
            var res = await _appointmentService.DoctorAcceptAppointment(doctorId, id);
            return StatusCode(res.statusCode, res);
        }

        [Authorize(Roles = "ROLE_PATIENT, ROLE_ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var role = User.FindFirstValue("Role");
            var userId = Convert.ToInt32(User.FindFirstValue("UserId"));
            var res = await _appointmentService.DeleteAppointment(id, role, userId);
            return StatusCode(res.statusCode, res);
        }

        [Authorize(Roles = "ROLE_DOCTOR")]
        [HttpPut("doctor-report/{id}")]
        public async Task<IActionResult> DoctorReportAppointment(int id)
        {
            var doctorId = Convert.ToInt32(User.FindFirstValue("DoctorId"));
            var res = await _appointmentService.DoctorReportAppointment(doctorId, id);
            return StatusCode(res.statusCode, res);
        }

        [Authorize(Roles = "ROLE_ADMIN")]
        [HttpPut("handle-report/{id}")]
        public async Task<IActionResult> AdminHandleReport(int id,[FromBody] string violator)
        {
            var res = await _appointmentService.AdminHandleReport(id, violator);
            return StatusCode(res.statusCode, res);
        }
        [Authorize(Roles = "ROLE_PATIENT")]
        [HttpPost("rate/{id}")]
        public async Task<IActionResult> PatientRateAppointment(int id, [FromBody] RateDto rate)
        {
            var patientId = Convert.ToInt32(User.FindFirstValue("UserId"));
            var res = await _appointmentService.PatientRateAppointment(id, patientId, rate);
            return StatusCode(res.statusCode, res);
        }
    }
}
