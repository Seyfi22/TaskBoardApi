using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TaskBoardApi.Data;
using TaskBoardApi.DTOs.Task;
using TaskBoardApi.Exceptions;
using TaskBoardApi.Model.Entities;
using TaskBoardApi.Services.Interfaces;

namespace TaskBoardApi.Services.Implementations
{
    public class TaskService : GenericService<TaskDto, TaskItem>, ITaskService
    {
        private readonly ILogger<TaskService> _logger;

        public TaskService(TaskBoardDbContext context, IMapper mapper, ILogger<TaskService> logger)
            : base(context, mapper)
        {
            _logger = logger;
        }

        public override async Task<List<TaskDto>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all tasks");

            return await _context.Tasks
                .Include(t => t.User)
                .ProjectTo<TaskDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public override async Task<TaskDto> GetByIdAsync(int id)
        {
            _logger.LogInformation("Getting task by id {id}", id);

            var task = await _context.Tasks
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                _logger.LogWarning("Task with id {id} not found", id);
                throw new NotFoundException($"Task with id {id} not found.");
            }

            _logger.LogInformation("Task with id {id} retrieved successfully", id);

            return _mapper.Map<TaskDto>(task);
        }

        public async Task<TaskDto> CreateAsync(CreateTaskDto createTaskDto)
        {
            if (createTaskDto == null)
            {
                _logger.LogWarning("CreateTaskDto is null");
                throw new BadRequestException("Task data cannot be null.");
            }

            _logger.LogInformation("Creating task assigned to UserId {userId}", createTaskDto.UserId);

            var userExists = await _context.Users.AnyAsync(u => u.Id == createTaskDto.UserId);

            if (!userExists)
            {
                _logger.LogWarning("Cannot create task. UserId {userId} does not exist", createTaskDto.UserId);
                throw new BadRequestException("Cannot create task. Provided UserId does not exist.");
            }

            var task = _mapper.Map<TaskItem>(createTaskDto);

            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Task created successfully with id {id}", task.Id);

            return _mapper.Map<TaskDto>(task);
        }

        public async Task<TaskDto> UpdateAsync(int id, UpdateTaskDto updateTaskDto)
        {
            _logger.LogInformation("Updating task with id {id}", id);

            if (updateTaskDto == null)
            {
                _logger.LogWarning("UpdateTaskDto is null for id {id}", id);
                throw new BadRequestException("Update data cannot be null.");
            }

            var task = await _context.Tasks
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                _logger.LogWarning("Task with id {id} not found", id);
                throw new NotFoundException($"Task with id {id} not found.");
            }

            if (updateTaskDto.UserId.HasValue)
            {
                var userExists = await _context.Users
                    .AnyAsync(u => u.Id == updateTaskDto.UserId.Value);

                if (!userExists)
                {
                    _logger.LogWarning("Task update failed. Provided UserId {userId} does not exist", updateTaskDto.UserId.Value);

                    throw new BadRequestException("Cannot update task. Provided UserId does not exist.");
                }
            }

            _mapper.Map(updateTaskDto, task);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Task with id {id} updated successfully", id);

            return _mapper.Map<TaskDto>(task);
        }
    }
}
