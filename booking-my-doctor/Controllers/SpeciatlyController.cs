using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using booking_my_doctor.Services;
using Microsoft.AspNetCore.Mvc;

namespace booking_my_doctor.Controllers
{
    [Route("api/speciatly")]
    [ApiController]
    public class SpeciatlyController : Controller
    {
        private readonly ISpeciatlyService _speciatlyService;
        public SpeciatlyController(ISpeciatlyService speciatlyService)
        {
            _speciatlyService = speciatlyService;
        }
        [HttpGet]
        public async Task<IActionResult> GetSpeciatlys(int? page = 0, int? pageSize = int.MaxValue, string? keyword = null, string? sortColumn = "Id") 
        {
            var resData = await _speciatlyService.GetSpeciatlys(page, pageSize, keyword, sortColumn);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSpeciatlyById(int id)
        {
            var resData = await _speciatlyService.GetSpeciatlyById(id);
            return StatusCode(resData.statusCode, resData);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSpeciatly([FromBody] SpeciatlyDto speciatlyDto)
        {
            var resData = await _speciatlyService.CreateSpeciatly(speciatlyDto);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpeciatly(int id, [FromBody] SpeciatlyDto speciatlyDto)
        {
            var resData = await _speciatlyService.UpdateSpeciatly(id, speciatlyDto);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpeciatly(int id)
        {
            var resData = await _speciatlyService.DeleteSpeciatly(id);
            return StatusCode(resData.statusCode, resData);
        }
    }
}
