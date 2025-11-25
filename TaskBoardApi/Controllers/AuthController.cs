using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskBoardApi.Data;
using TaskBoardApi.DTOs.Auth;
using TaskBoardApi.Exceptions;
using TaskBoardApi.Services.Interfaces;
using TaskBoardApi.Services.Interfaces.Jwt;

namespace TaskBoardApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var token = await _authService.LoginAsync(loginDto);

            return Ok(new
            {
                Token = token,
                Message = "Login successful"
            });
        }
    }
}
