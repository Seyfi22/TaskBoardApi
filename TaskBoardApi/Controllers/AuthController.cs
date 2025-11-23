using Microsoft.AspNetCore.Mvc;
using TaskBoardApi.DTOs.Auth;
using TaskBoardApi.Services.Interfaces.Jwt;
using TaskBoardApi.Data;
using Microsoft.EntityFrameworkCore;
using TaskBoardApi.Exceptions;

namespace TaskBoardApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TaskBoardDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthController(TaskBoardDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
                throw new NotFoundException("User not found.");

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                throw new BadRequestException("Password is incorrect.");

            var token = _tokenService.CreateToken(user.Id, user.Email, user.Role.ToString());

            return Ok(new
            {
                Token = token,
                Message = "Login successful"
            });
        }
    }
}
