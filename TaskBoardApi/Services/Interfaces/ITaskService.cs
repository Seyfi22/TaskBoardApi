using TaskBoardApi.DTOs.Task;

namespace TaskBoardApi.Services.Interfaces
{
    public interface ITaskService : IGenericService<TaskDto>
    {
        Task<TaskDto> CreateAsync(CreateTaskDto createTaskDto);
        Task<TaskDto> UpdateAsync(int id, UpdateTaskDto updateTaskDto);
    }
}
