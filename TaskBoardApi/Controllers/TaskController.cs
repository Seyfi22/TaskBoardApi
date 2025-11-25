using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBoardApi.DTOs.Task;
using TaskBoardApi.Exceptions;
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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null)
                throw new UnauthorizedException("User info not found.");

            int currentUserId = int.Parse(userIdClaim);

            var tasks = await _taskService.GetAllAsync();

            if (role == "Admin")
                return Ok(tasks);

            var userTasks = tasks.Where(t => t.User == currentUserId);

            return Ok(userTasks);
        }

        [HttpGet("{id}", Name = "GetTaskById")]
        [Authorize]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null)
                throw new UnauthorizedException("User info not found.");

            var currentUserId = int.Parse(userIdClaim);

            var task = await _taskService.GetByIdAsync(id);

            if (task == null)
                throw new NotFoundException($"Task with id {id} not found.");

            if (role != "Admin" && task.User != currentUserId)
                throw new ForbiddenException("You are not allowed to access this task.");

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
                throw new UnauthorizedException("User info not found.");

            int currentUserId = int.Parse(userIdClaim);

            if (role != "Admin" && createTaskDto.UserId != currentUserId)
                throw new ForbiddenException("You cannot create a task for another user.");

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
                throw new UnauthorizedException("User info not found.");

            int currentUserId = int.Parse(userIdClaim);

            var task = await _taskService.GetByIdAsync(id);
            if (task == null)
                throw new NotFoundException($"Task with id {id} not found.");

            if (role != "Admin" && task.User != currentUserId)
                throw new ForbiddenException("You cannot update this task.");

            if (role != "Admin" && updateTaskDto.UserId.HasValue && updateTaskDto.UserId != currentUserId)
                throw new ForbiddenException("You cannot transfer this task to another user.");

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
