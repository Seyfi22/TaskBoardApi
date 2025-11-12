namespace TaskBoardApi.DTOs.User
{
    public class TaskInUserDto
    {
        public int Id { get; set; } 
        public string Title { get; set; }
        public DateTime Deadline { get; set; }
        public bool IsCompleted { get; set; }
    }
}
