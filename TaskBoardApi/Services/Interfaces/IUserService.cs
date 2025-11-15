using TaskBoardApi.DTOs.User;

namespace TaskBoardApi.Services.Interfaces
{
    public interface IUserService : IGenericService<UserDto>
    {
        Task<UserDto> CreateAsync(CreateUserDto createUserDto);
        Task<UserDto> UpdateAsync(int id, UpdateUserDto updateUserDto);
        Task<bool> IsEmailRegisteredByAnotherAccountAsync(int currentUserId, string email);
    }
}
