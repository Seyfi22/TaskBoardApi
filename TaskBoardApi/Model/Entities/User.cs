using TaskBoardApi.Model.Enums;

namespace TaskBoardApi.Model.Entities
{
    public class User
    {
        public User()
        {
            Tasks = new HashSet<TaskItem>();
        }

        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public Role Role { get; set; }

        public ICollection<TaskItem> Tasks { get; set; }
    }
}
