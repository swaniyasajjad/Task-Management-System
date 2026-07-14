using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int? CurrentStudentId()
        {
            return HttpContext.Session.GetInt32("StudentId");
        }

        public IActionResult Index(string search, string status, string priority, string category)
        {
            var studentId = CurrentStudentId();

            if (studentId == null)
                return RedirectToAction("Login", "Account");

            var tasks = _context.Tasks
                .Where(t => t.StudentId == studentId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                tasks = tasks.Where(t =>
                    t.Title.Contains(search) ||
                    (t.Description != null && t.Description.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (Enum.TryParse<TaskProgressStatus>(status, out var parsedStatus))
                {
                    tasks = tasks.Where(t => t.Status == parsedStatus);
                }
            }

            if (!string.IsNullOrWhiteSpace(priority))
            {
                if (Enum.TryParse<Priority>(priority, out var parsedPriority))
                {
                    tasks = tasks.Where(t => t.Priority == parsedPriority);
                }
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                if (Enum.TryParse<TaskCategory>(category, out var parsedCategory))
                {
                    tasks = tasks.Where(t => t.Category == parsedCategory);
                }
            }

            var result = tasks
                .OrderByDescending(t => t.CreatedAt)
                .ToList();

            return View(result);
        }

        public IActionResult Create()
        {
            if (CurrentStudentId() == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public IActionResult Create(TaskItem task)
        {
            var studentId = CurrentStudentId();

            if (studentId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(task);

            task.StudentId = studentId.Value;
            task.CreatedAt = DateTime.Now;
            task.Status = TaskProgressStatus.Pending;

            _context.Tasks.Add(task);
            _context.SaveChanges();

            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult Edit(int id)
        {
            var studentId = CurrentStudentId();

            if (studentId == null)
                return RedirectToAction("Login", "Account");

            var task = _context.Tasks
                .FirstOrDefault(t => t.Id == id && t.StudentId == studentId);

            if (task == null)
                return NotFound();

            return View(task);
        }

        [HttpPost]
        public IActionResult Edit(TaskItem task)
        {
            var studentId = CurrentStudentId();

            if (studentId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(task);

            var existingTask = _context.Tasks
                .FirstOrDefault(t => t.Id == task.Id && t.StudentId == studentId);

            if (existingTask == null)
                return NotFound();

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.Priority = task.Priority;
            existingTask.Category = task.Category;
            existingTask.Status = task.Status;
            existingTask.Deadline = task.Deadline;

            _context.SaveChanges();

            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult Delete(int id)
        {
            var studentId = CurrentStudentId();

            if (studentId == null)
                return RedirectToAction("Login", "Account");

            var task = _context.Tasks
                .FirstOrDefault(t => t.Id == id && t.StudentId == studentId);

            if (task != null)
            {
                _context.Tasks.Remove(task);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult Complete(int id)
        {
            var studentId = CurrentStudentId();

            if (studentId == null)
                return RedirectToAction("Login", "Account");

            var task = _context.Tasks
                .FirstOrDefault(t => t.Id == id && t.StudentId == studentId);

            if (task != null)
            {
                task.Status = TaskProgressStatus.Completed;
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Dashboard");
        }
    }
}
