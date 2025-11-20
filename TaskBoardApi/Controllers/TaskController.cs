using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskBoardApi.DTOs.Task;
using TaskBoardApi.Services.Interfaces;

namespace TaskBoardApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var tasks = await _taskService.GetAllAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}", Name = "GetTaskById")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var task = await _taskService.GetByIdAsync(id);
           
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTaskDto createTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _taskService.CreateAsync(createTaskDto);

            return CreatedAtRoute("GetTaskById", new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var updatedTask = await _taskService.UpdateAsync(id, updateTaskDto);

            return Ok(updatedTask);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _taskService.DeleteAsync(id);

            return NoContent();
        }
    }
}
