using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int? studentId = HttpContext.Session.GetInt32("StudentId");

            if (studentId == null)
                return RedirectToAction("Login", "Account");

            var tasks = _context.Tasks
                .Where(t => t.StudentId == studentId)
                .OrderByDescending(t => t.CreatedAt)
                .ToList();

            ViewBag.Name = HttpContext.Session.GetString("StudentName");
            ViewBag.Total = tasks.Count;
            ViewBag.Pending = tasks.Count(t => t.Status == TaskProgressStatus.Pending);
            ViewBag.InProgress = tasks.Count(t => t.Status == TaskProgressStatus.InProgress);
            ViewBag.Completed = tasks.Count(t => t.Status == TaskProgressStatus.Completed);
            ViewBag.Incomplete = tasks.Count(t => t.Status == TaskProgressStatus.Incomplete);
            ViewBag.Urgent = tasks.Count(t => t.Priority == Priority.Urgent);

            return View(tasks);
        }

        public IActionResult Admin()
        {
            string? role = HttpContext.Session.GetString("Role");

            if (role != "Admin")
                return RedirectToAction("Login", "Account");

            ViewBag.TotalStudents = _context.Students.Count();
            ViewBag.TotalTasks = _context.Tasks.Count();
            ViewBag.CompletedTasks = _context.Tasks.Count(t => t.Status == TaskProgressStatus.Completed);
            ViewBag.PendingTasks = _context.Tasks.Count(t => t.Status == TaskProgressStatus.Pending);

            var allTasks = _context.Tasks
                .OrderByDescending(t => t.CreatedAt)
                .ToList();

            return View(allTasks);
        }
    }
}
