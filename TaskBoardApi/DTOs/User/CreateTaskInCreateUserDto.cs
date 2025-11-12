namespace TaskBoardApi.DTOs.User
{
    public class CreateTaskInCreateUserDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime Deadline { get; set; }
        public bool IsCompleted { get; set; } = false;
    }
}
