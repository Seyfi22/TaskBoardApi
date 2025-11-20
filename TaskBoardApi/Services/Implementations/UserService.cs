using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TaskBoardApi.Data;
using TaskBoardApi.DTOs.User;
using TaskBoardApi.Exceptions;
using TaskBoardApi.Model.Entities;
using TaskBoardApi.Services.Interfaces;

namespace TaskBoardApi.Services.Implementations
{
    public class UserService : GenericService<UserDto, User>, IUserService
    {
        private readonly ILogger<UserService> _logger;

        public UserService(TaskBoardDbContext context, IMapper mapper, ILogger<UserService> logger) : base(context, mapper) 
        {
            _logger = logger;
        }

        public override async Task<List<UserDto>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Tasks)
                .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public override async Task<UserDto> GetByIdAsync(int id)
        {
            _logger.LogInformation("Getting user by id {id}", id);

            var user = await _context.Users
                .Include(u => u.Tasks)
                .FirstOrDefaultAsync(u => u.Id == id);

            if(user == null)
            {
                throw new NotFoundException($"User with id {id} not found.");
            }

            _logger.LogInformation("User with id {id} retrieved successfully", id);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateAsync(CreateUserDto createUserDto)
        {
            if(createUserDto == null)
            {
                _logger.LogWarning("CreateUserDto is null");
                throw new BadRequestException("User data cannot be null.");
            }

            _logger.LogInformation("Creating user with email {email}", createUserDto.Email);

            var emailExist = await _context.Users.AnyAsync(u => u.Email == createUserDto.Email);

            if(emailExist)
            {
                _logger.LogWarning("User creation failed. Email {email} already exists", createUserDto.Email);
                throw new BadRequestException("Email is already registered.");
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

            _logger.LogInformation("User created successfully with id {id}", user.Id);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateAsync(int id, UpdateUserDto updateUserDto)
        {
            _logger.LogInformation("Updating user with id {id}", id);

            if (updateUserDto == null)
            {
                _logger.LogWarning("UpdateUserDto is null for id {id}", id);
                throw new BadRequestException("Update data cannot be null.");
            }

            var user = await _context.Users
                .Include(u => u.Tasks)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                throw new NotFoundException($"User with id {id} not found.");
            }

            if(updateUserDto.Email != null)
            {
                if(await IsEmailRegisteredByAnotherAccountAsync(id, updateUserDto.Email))
                {
                    _logger.LogWarning("User {id} update failed. Email belongs to another account", id);
                    throw new BadRequestException("This email belongs to another user.");
                }
            }

            _mapper.Map(updateUserDto, user);

            await _context.SaveChangesAsync();

            _logger.LogInformation("User with id {id} updated successfully", id);

            return _mapper.Map<UserDto>(user);

        }

        public async Task<bool> IsEmailRegisteredByAnotherAccountAsync(int currentUserId, string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email.ToLower().Trim() == email.ToLower().Trim() && u.Id != currentUserId);
        }
    }
}
