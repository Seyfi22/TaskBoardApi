using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TaskBoardApi.Data;
using TaskBoardApi.DTOs.Task;
using TaskBoardApi.Model.Entities;
using TaskBoardApi.Services.Interfaces;

namespace TaskBoardApi.Services.Implementations
{
    public class TaskService : GenericService<TaskDto, TaskItem>
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
    }
}
