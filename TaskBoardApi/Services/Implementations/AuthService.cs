using Microsoft.EntityFrameworkCore;
using TaskBoardApi.Data;
using TaskBoardApi.DTOs.Auth;
using TaskBoardApi.Exceptions;
using TaskBoardApi.Services.Interfaces;
using TaskBoardApi.Services.Interfaces.Jwt;

namespace TaskBoardApi.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly TaskBoardDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthService(TaskBoardDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
                throw new NotFoundException("User not found.");

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                throw new BadRequestException("Password is incorrect.");

            return _tokenService.CreateToken(user.Id, user.Email, user.Role.ToString());
        }
    }
}
