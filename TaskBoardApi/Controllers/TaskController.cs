using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task<IActionResult> GetAllAsync()
        {
            var tasks = await _taskService.GetAllAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}", Name = "GetTaskById")]
        [Authorize]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null)
            {
                return Unauthorized("User info not found in token.");
            }

            var currentUserId = int.Parse(userIdClaim);

            // Task-ı DB-dən oxuyuruq
            var task = await _taskService.GetByIdAsync(id);

            // Task tapılmadısa
            if (task == null)
            {
                return NotFound($"Task with id {id} not found.");
            }

            // Əgər admin deyilsə və task başqa user-ə aiddirsə → Forbid
            if (role != "Admin" && task.User != currentUserId)
            {
                return Forbid("You are not allowed to access this task.");
            }

            return Ok(task);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTaskDto createTaskDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null)
                return Unauthorized("User info not found.");

            int currentUserId = int.Parse(userIdClaim);

            // User yalnız özünə aid task yarada bilər
            if (role != "Admin" && createTaskDto.UserId != currentUserId)
                return Forbid("You cannot create tasks for another user.");

            var result = await _taskService.CreateAsync(createTaskDto);

            return CreatedAtRoute("GetTaskById", new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null)
                return Unauthorized("User info not found.");

            int currentUserId = int.Parse(userIdClaim);

            var task = await _taskService.GetByIdAsync(id);
            if (task == null)
                return NotFound($"Task with id {id} not found.");

            // User yalnız öz taskını update edə bilər
            if (role != "Admin" && task.User != currentUserId)
                return Forbid("You cannot update this task.");

            // User yalnız özünə aid etmək üçün dəyişə bilər (başqa userə köçürə bilməz)
            if (role != "Admin" && updateTaskDto.UserId.HasValue && updateTaskDto.UserId != currentUserId)
                return Forbid("You cannot transfer this task to another user.");

            var updatedTask = await _taskService.UpdateAsync(id, updateTaskDto);

            return Ok(updatedTask);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _taskService.DeleteAsync(id);

            return NoContent();
        }
    }
}
