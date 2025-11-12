using TaskBoardApi.Model.Enums;

namespace TaskBoardApi.DTOs.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public ICollection<TaskInUserDto> Tasks { get; set; } = new List<TaskInUserDto>();
    }
}
