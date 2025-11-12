namespace TaskBoardApi.DTOs.User
{
    public class CreateUserDto
    {
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "User";

        public ICollection<CreateTaskInCreateUserDto> Tasks { get; set; } = new List<CreateTaskInCreateUserDto>();
    }
}
