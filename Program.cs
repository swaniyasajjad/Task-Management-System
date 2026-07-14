using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSession();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    db.Database.Migrate();

    if (!db.Students.Any(s => s.Email == "admin@gmail.com"))
    {
        db.Students.Add(new Student
        {
            FullName = "Admin",
            Email = "admin@gmail.com",
            PasswordHash = PasswordHelper.HashPassword("admin123"),
            Role = UserRole.Admin
        });

        db.SaveChanges();
    }

    var expiredTasks = db.Tasks
        .Where(t => t.Status == TaskProgressStatus.Pending &&
                    t.Deadline < DateTime.Now)
        .ToList();

    foreach (var task in expiredTasks)
    {
        task.Status = TaskProgressStatus.Incomplete;
    }

    db.SaveChanges();
}

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
