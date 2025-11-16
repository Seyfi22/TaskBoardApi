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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var task = await _taskService.GetByIdAsync(id);
            
            if(task == null)
            {
                return NotFound($"Task with id {id} not found.");
            }

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

            if (result == null)
            {
                return BadRequest("Task cannot be created.");
            }

            return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedTask = await _taskService.UpdateAsync(id, updateTaskDto);

                if (updatedTask == null)
                {
                    return NotFound($"Task with id {id} not found.");
                }

                return Ok(updatedTask);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deletedTask = await _taskService.DeleteAsync(id);

            if (!deletedTask)
            {
                return NotFound($"Task with id {id} not found.");
            }

            return NoContent();
        }
    }
}
