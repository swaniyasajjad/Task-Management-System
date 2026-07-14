using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public enum Priority
    {
        Low,
        Medium,
        High,
        Urgent
    }

    public enum TaskProgressStatus
    {
        Pending,
        InProgress,
        Completed,
        Incomplete
    }

    public enum TaskCategory
    {
        Study,
        Assignment,
        Exam,
        Project,
        Personal,
        Other
    }

    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public Priority Priority { get; set; }

        [Required]
        public TaskProgressStatus Status { get; set; } = TaskProgressStatus.Pending;

        [Required]
        public TaskCategory Category { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int StudentId { get; set; }

        public Student? Student { get; set; }
    }
}
