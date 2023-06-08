using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using booking_my_doctor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApiApp.Models;

namespace booking_my_doctor.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        //[Authorize(Roles = "ROLE_ADMIN")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto user)
        {

            var resData = await _userService.CreateUser(user);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(int? page = null, int? pageSize = null, string? name = null, string? roleName = null, string? sortColumn = "Id")
        {

            var resData = await _userService.GetAllUsers(page, pageSize, name, sortColumn, roleName);
            return Ok(resData);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var resData = await _userService.GetUserById(id);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> EditUser(int id, [FromBody] UserUpdateDTO userUpdateDTO)
        {
            var resData = await _userService.UpdateUser(id, userUpdateDTO);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var resData = await _userService.DeleteUser(id);
            return StatusCode(resData.statusCode, resData);
        }
    }
}
