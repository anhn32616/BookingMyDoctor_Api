using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using booking_my_doctor.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWebApiApp.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace booking_my_doctor.Controllers
{
    [Route("api/doctor")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }
        // GET: api/<DoctorController>
        [HttpGet]
        public async Task<IActionResult> GetDoctors(int? page = 0, int? pageSize = int.MaxValue, string? keyword = null, string? sortColumn = "Id")
        {
            var resData = await _doctorService.GetDoctors(page, pageSize, keyword, sortColumn);
            return StatusCode(resData.statusCode, resData);
        }

        // GET api/<DoctorController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            var resData = await _doctorService.GetDoctorById(id);
            return StatusCode(resData.statusCode, resData);
        }

        // POST api/<DoctorController>
        [HttpPost]
        public async Task<IActionResult> CreateDoctor([FromBody] DoctorCreateDto doctorCreateDto)
        {
            var resData = await _doctorService.CreateDoctor(doctorCreateDto);
            return StatusCode(resData.statusCode, resData);
        }

        // PUT api/<DoctorController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] DoctorUpdateDto doctorUpdateDto)
        {
            var resData = await _doctorService.UpdateDoctor(id, doctorUpdateDto);
            return StatusCode(resData.statusCode, resData);
        }


        // DELETE api/<DoctorController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var resData = await _doctorService.DeleteDoctor(id);
            return StatusCode(resData.statusCode, resData);
        }
    }
}
