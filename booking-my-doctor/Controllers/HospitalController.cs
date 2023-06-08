using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using booking_my_doctor.Services;
using Microsoft.AspNetCore.Mvc;

namespace booking_my_doctor.Controllers
{
    [Route("api/hospital")]
    [ApiController]
    public class HospitalController : Controller
    {
        private readonly IHospitalService _hospitalService;
        public HospitalController(IHospitalService hospitalService)
        {
            _hospitalService = hospitalService;
        }
        [HttpGet]
        public async Task<IActionResult> GetHospitals(int? page = 0, int? pageSize = int.MaxValue, string? keyword = null, string? sortColumn = "Id") 
        {
            var resData = await _hospitalService.GetHospitals(page, pageSize, keyword, sortColumn);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHospitalById(int id)
        {
            var resData = await _hospitalService.GetHospitalById(id);
            return StatusCode(resData.statusCode, resData);
        }

        [HttpPost]
        public async Task<IActionResult> CreateHospital([FromBody] HospitalDto hospitalDto)
        {
            var resData = await _hospitalService.CreateHospital(hospitalDto);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHospital(int id, [FromBody] HospitalDto hospitalDto)
        {
            var resData = await _hospitalService.UpdateHospital(id, hospitalDto);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHospital(int id)
        {
            var resData = await _hospitalService.DeleteHospital(id);
            return StatusCode(resData.statusCode, resData);
        }
    }
}
