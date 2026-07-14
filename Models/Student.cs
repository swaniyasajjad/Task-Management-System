using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public enum UserRole
    {
        Student,
        Admin
    }

    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.Student;

        public List<TaskItem>? Tasks { get; set; }
    }
}
