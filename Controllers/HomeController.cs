using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Models;

namespace ToDoApp.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;

    public HomeController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var user = await _userManager.GetUserAsync(User);
        var userId = user?.Id;

        var totalItems = await _dbContext.ToDoItems
            .CountAsync(t => t.UserId == userId);

        var pageSize = 7;
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var items = await _dbContext.ToDoItems
            .Where(t => t.UserId == userId)
            .OrderBy(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var model = new IndexModel
        {
            ShowComplete = false,
            TodoItems = items,
            CurrentPage = page,
            TotalPages = totalPages,
            PageSize = pageSize
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ShowComplete(bool showComplete, int page = 1)
    {
        var user = await _userManager.GetUserAsync(User);
        var userId = user?.Id;

        var totalItems = await _dbContext.ToDoItems
           .CountAsync(t => t.UserId == userId && (showComplete || !t.Complete));

        var pageSize = 10; // Number of items per page
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var items = await _dbContext.ToDoItems
            .Where(t => t.UserId == userId && (showComplete || !t.Complete))
            .OrderBy(t => t.Id) // Optional: Order by a property
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var model = new IndexModel
        {
            ShowComplete = showComplete,
            TodoItems = items,
            CurrentPage = page,
            TotalPages = totalPages,
            PageSize = pageSize
        };

        return View("Index", model);
    }

    [HttpPost]
    public async Task<IActionResult> MarkItem(int id)
    {
        var item = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == id);
        if (item == null)
        {
            TempData["ErrorMessage"] = "Item not found.";
            return RedirectToAction("Index");
        }

        item.Complete = !item.Complete;
        await _dbContext.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddItem(string task)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(task))
            {
                TempData["ErrorMessage"] = "Task cannot be empty.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;

            var newItem = new TodoItem
            {
                Task = task,
                Complete = false,
                Owner = User.Identity.Name,
                Category = "Shkolle",
                LabelColor = "Red",
                UserId = userId
            };

            await _dbContext.ToDoItems.AddAsync(newItem);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log the exception (e.g., using ILogger)
            TempData["ErrorMessage"] = "An error occurred while adding the task.";
        }

        return RedirectToAction("Index");
    }
}