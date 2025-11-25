using TaskBoardApi.DTOs.Auth;

namespace TaskBoardApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginDto loginDto);
    }
}
