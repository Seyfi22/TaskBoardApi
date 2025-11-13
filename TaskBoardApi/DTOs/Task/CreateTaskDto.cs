namespace TaskBoardApi.DTOs.Task
{
    public class CreateTaskDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime Deadline { get; set; }
        public bool IsCompleted { get; set; } = false;

        public int UserId { get; set; }
    }
}
