using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreSystem.Models;
using System.Security.Claims;
using testproject.Models;

public class AccountController : Controller
{

    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = _context.Users
            .Include(u => u.Post)
            .FirstOrDefault(u => u.Username == username && u.PasswordHash == password);

        if (user == null)
        {
            ModelState.AddModelError("", "Неправильный логин или пароль");
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Post.Name),
            new Claim("EmployeeId", user.EmployeeId.ToString()) // Добавляем EmployeeId в claims
        };

        var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
        var authProperties = new AuthenticationProperties { };

        await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    public IActionResult Register()
    {
        // Получаем список сотрудников для выпадающего списка
        ViewBag.EmployeeId = new SelectList(_context.Employees.Include(e => e.Post), "Id", "FirstName");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string username, string password, int employeeId)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("", "Логин и пароль обязательны.");
            ViewBag.EmployeeId = new SelectList(_context.Employees.Include(e => e.Post), "Id", "FirstName", employeeId);
            return View();
        }

        // Проверяем, существует ли пользователь
        if (_context.Users.Any(u => u.Username == username))
        {
            ModelState.AddModelError("", "Пользователь с таким логином уже существует.");
            ViewBag.EmployeeId = new SelectList(_context.Employees.Include(e => e.Post), "Id", "FirstName", employeeId);
            return View();
        }

        // Извлекаем сотрудника и его должность
        var employee = await _context.Employees.Include(e => e.Post).FirstOrDefaultAsync(e => e.Id == employeeId);
        if (employee == null)
        {
            ModelState.AddModelError("", "Выбранный сотрудник не найден.");
            ViewBag.EmployeeId = new SelectList(_context.Employees.Include(e => e.Post), "Id", "FirstName", employeeId);
            return View();
        }

        // Добавляем нового пользователя
        var user = new User
        {
            Username = username,
            PasswordHash = password, // Здесь можно добавить хэширование
            EmployeeId = employee.Id,
            PostId = employee.PostId // Устанавливаем должность, связанную с сотрудником
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Login");
    }

    // Новый метод для получения текущего пользователя
    [HttpGet]
    [Authorize] // Убедитесь, что только авторизованные пользователи могут обращаться к этому методу
    public IActionResult GetCurrentUser()
    {
        var user = HttpContext.User;
        if (user.Identity == null || !user.Identity.IsAuthenticated)
        {
            return Unauthorized(new { message = "Пользователь не авторизован" });
        }

        // Извлекаем EmployeeId из claims
        var employeeIdClaim = user.Claims.FirstOrDefault(c => c.Type == "EmployeeId");
        if (employeeIdClaim == null)
        {
            return BadRequest(new { message = "EmployeeId не найден" });
        }

        var employeeId = int.Parse(employeeIdClaim.Value);

        // Получаем пользователя и его данные из базы
        var currentUser = _context.Users
            .Include(u => u.Post)
            .FirstOrDefault(u => u.EmployeeId == employeeId);

        if (currentUser == null)
        {
            return NotFound(new { message = "Пользователь не найден" });
        }

        return Ok(new
        {
            Username = currentUser.Username,
            EmployeeId = currentUser.EmployeeId,
            Role = currentUser.Post.Name
        });
    }
}
