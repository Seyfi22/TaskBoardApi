using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TaskBoardApi.Data;
using TaskBoardApi.DTOs.User;
using TaskBoardApi.Model.Entities;
using TaskBoardApi.Services.Interfaces;

namespace TaskBoardApi.Services.Implementations
{
    public class UserService : GenericService<UserDto, User>, IUserService
    {
        public UserService(TaskBoardDbContext context, IMapper mapper) : base(context, mapper) { }

        public override async Task<List<UserDto>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Tasks)
                .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public override async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Tasks)
                .FirstOrDefaultAsync(u => u.Id == id);

            if(user == null)
            {
                return null;
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateAsync(CreateUserDto createUserDto)
        {
            if(createUserDto == null)
            {
                return null;
            }

            var emailExist = await _context.Users.AnyAsync(u => u.Email == createUserDto.Email);

            if(emailExist)
            {
                return null;
            }

            var user = _mapper.Map<User>(createUserDto);

            if(createUserDto.Tasks != null && createUserDto.Tasks.Any())
            {
                user.Tasks = createUserDto.Tasks
                    .Select(t => _mapper.Map<TaskItem>(t))
                    .ToList();
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateAsync(int id, UpdateUserDto updateUserDto)
        {
            if (updateUserDto == null)
            {
                return null;
            }

            var user = await _context.Users
                .Include(u => u.Tasks)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return null;
            }

            if(updateUserDto.Email != null)
            {
                if(await IsEmailRegisteredByAnotherAccountAsync(id, updateUserDto.Email))
                {
                    throw new Exception("This email belongs to another user.");
                }
            }

            _mapper.Map(updateUserDto, user);

            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);

        }

        public async Task<bool> IsEmailRegisteredByAnotherAccountAsync(int currentUserId, string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email.ToLower().Trim() == email.ToLower().Trim() && u.Id != currentUserId);
        }
    }
}
