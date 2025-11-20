namespace TaskBoardApi.DTOs.User
{
    public class UpdateUserDto
    {
        public string? Fullname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
    }
}
