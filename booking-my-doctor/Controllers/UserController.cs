using booking_my_doctor.DTOs;
using booking_my_doctor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize(Roles = "ROLE_ADMIN")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto user)
        {

            var resData = await _userService.CreateUser(user);
            return StatusCode(resData.statusCode, resData);
        }
        [Authorize(Roles = "ROLE_ADMIN, ROLE_DOCTOR")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(int? page = null, int? pageSize = null, string? name = null, string? roleName = null, string? sortColumn = "Id")
        {

            var resData = await _userService.GetAllUsers(page, pageSize, name, sortColumn, roleName);
            return Ok(resData);
        }
        [Authorize(Roles = "ROLE_ADMIN")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var resData = await _userService.GetUserById(id);
            return StatusCode(resData.statusCode, resData);
        }
        [Authorize(Roles = "ROLE_ADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditUser(int id, [FromBody] UserUpdateDTO userUpdateDTO)
        {
            var resData = await _userService.UpdateUser(id, userUpdateDTO);
            return StatusCode(resData.statusCode, resData);
        }
        [Authorize(Roles = "ROLE_ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var resData = await _userService.DeleteUser(id);
            return StatusCode(resData.statusCode, resData);
        }
        [Authorize(Roles = "ROLE_ADMIN")]
        [HttpPut("open-close/{id}")]
        public async Task<IActionResult> OpenCloseUser(int id)
        {
            var resData = await _userService.OpenCloseUser(id);
            return StatusCode(resData.statusCode, resData);
        }
        [Authorize()]
        [HttpGet("profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = Convert.ToInt32(User.FindFirstValue("UserId"));
            var resData = await _userService.GetUserById(userId);
            return StatusCode(resData.statusCode, resData);
        }
        [Authorize()]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateDTO userUpdateDTO)
        {
            var userId = Convert.ToInt32(User.FindFirstValue("UserId"));
            var resData = await _userService.UpdateUser(userId, userUpdateDTO);
            return StatusCode(resData.statusCode, resData);
        }
    }
}
