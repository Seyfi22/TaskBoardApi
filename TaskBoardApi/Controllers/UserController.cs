using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBoardApi.DTOs.User;
using TaskBoardApi.Exceptions;
using TaskBoardApi.Services.Interfaces;

namespace TaskBoardApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAsync()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}", Name = "GetUserById")]
        [Authorize]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null)
                throw new UnauthorizedException("User info not found.");

            var currentUserId = int.Parse(userIdClaim);

            if (role != "Admin" && currentUserId != id)
                throw new ForbiddenException("You are not allowed to access this user.");

            var user = await _userService.GetByIdAsync(id);

            return Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.CreateAsync(createUserDto);

            return CreatedAtRoute("GetUserById", new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedUser = await _userService.UpdateAsync(id, updateUserDto);

            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }
    }
}
