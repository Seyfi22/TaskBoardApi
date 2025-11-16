using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TaskBoardApi.Data;
using TaskBoardApi.DTOs.Task;
using TaskBoardApi.Model.Entities;
using TaskBoardApi.Services.Interfaces;

namespace TaskBoardApi.Services.Implementations
{
    public class TaskService : GenericService<TaskDto, TaskItem>, ITaskService
    {
        public TaskService(TaskBoardDbContext context, IMapper mapper) : base (context, mapper) { }

        public override async Task<List<TaskDto>> GetAllAsync()
        {
            return await _context.Tasks
                .Include(t => t.User)
                .ProjectTo<TaskDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public override async Task<TaskDto> GetByIdAsync(int id)
        {
            var task = await _context.Tasks
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if(task == null)
            {
                return null;
            }

            return _mapper.Map<TaskDto>(task);
        }

        public async Task<TaskDto> CreateAsync(CreateTaskDto createTaskDto)
        {
            if(createTaskDto == null)
            {
                return null;
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == createTaskDto.UserId);

            if(!userExists)
            {
                throw new Exception("Cannot create task. User with the given ID does not exist.");
            }

            var task = _mapper.Map<TaskItem>(createTaskDto);

            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            return _mapper.Map<TaskDto>(task);
        }

        public async Task<TaskDto> UpdateAsync(int id, UpdateTaskDto updateTaskDto)
        {
            if(updateTaskDto == null)
            {
                return null;
            }

            var task = await _context.Tasks
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if(task == null)
            {
                return null;
            }

            if (updateTaskDto.UserId.HasValue)
            {
                var userExists = await _context.Users
                    .AnyAsync(u => u.Id == updateTaskDto.UserId.Value);

                if (!userExists)
                    throw new Exception("Cannot update task. Provided UserId does not exist.");
            }

            _mapper.Map(updateTaskDto, task);

            await _context.SaveChangesAsync();

            return _mapper.Map<TaskDto>(task);
        }
    }
}
