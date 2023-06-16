using booking_my_doctor.DTOs;
using booking_my_doctor.Services;
using Microsoft.AspNetCore.Mvc;

namespace booking_my_doctor.Controllers
{
    [Route("api/clinic")]
    [ApiController]
    public class ClinicController : Controller
    {
        private readonly IClinicService _clinicService;
        public ClinicController(IClinicService clinicService)
        {
            _clinicService = clinicService;
        }
        [HttpGet]
        public async Task<IActionResult> GetClinics(int? page = 0, int? pageSize = int.MaxValue, string? keyword = null, string? sortColumn = "Id") 
        {
            var resData = await _clinicService.GetClinics(page, pageSize, keyword, sortColumn);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClinicById(int id)
        {
            var resData = await _clinicService.GetClinicById(id);
            return StatusCode(resData.statusCode, resData);
        }

        [HttpPost]
        public async Task<IActionResult> CreateClinic([FromBody] ClinicDto clinicDto)
        {
            var resData = await _clinicService.CreateClinic(clinicDto);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClinic(int id, [FromBody] ClinicDto clinicDto)
        {
            var resData = await _clinicService.UpdateClinic(id, clinicDto);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClinic(int id)
        {
            var resData = await _clinicService.DeleteClinic(id);
            return StatusCode(resData.statusCode, resData);
        }
    }
}
