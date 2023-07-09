using booking_my_doctor.DTOs;
using booking_my_doctor.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace booking_my_doctor.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            try
            {
                return Ok(await _authService.Register(registerUserDto));
            } catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            try
            {
                var resData = await _authService.Login(userLoginDto);
                return StatusCode(resData.statusCode, resData);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("verified")]
        public async Task<IActionResult> VerifiedEmail(string token)
        {
            var resData = await _authService.VerifiedEmail(token);
            return StatusCode(resData.statusCode, resData);
        }
        [HttpGet("sendMailVerified")]
        public async Task<IActionResult> SendMailVerified(string email)
        {
            var resData = await _authService.SendMailVerified(email);
            return StatusCode(resData.statusCode, resData);
        }

        [HttpPost("forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var res = await _authService.ForgotPassword(email);
            return StatusCode(res.statusCode, res);
        }
    }
}
