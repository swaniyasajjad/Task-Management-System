using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Data;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string fullName, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "All fields are required.";
                return View();
            }

            bool exists = _context.Students.Any(s => s.Email == email);

            if (exists)
            {
                ViewBag.Error = "Email already registered.";
                return View();
            }

            var student = new Student
            {
                FullName = fullName,
                Email = email,
                PasswordHash = PasswordHelper.HashPassword(password),
                Role = UserRole.Student
            };

            _context.Students.Add(student);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Students.FirstOrDefault(s => s.Email == email);

            if (user == null || !PasswordHelper.VerifyPassword(password, user.PasswordHash))
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            HttpContext.Session.SetInt32("StudentId", user.Id);
            HttpContext.Session.SetString("StudentName", user.FullName);
            HttpContext.Session.SetString("Role", user.Role.ToString());

            if (user.Role == UserRole.Admin)
                return RedirectToAction("Admin", "Dashboard");

            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
