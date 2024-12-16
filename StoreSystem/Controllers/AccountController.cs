using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreSystem.Models;
using System.Security.Claims;
using testproject.Models;
using StoreSystem.ViewModels;

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
            new Claim("EmployeeId", user.EmployeeId.ToString())
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

       
        if (_context.Users.Any(u => u.Username == username))
        {
            ModelState.AddModelError("", "Пользователь с таким логином уже существует.");
            ViewBag.EmployeeId = new SelectList(_context.Employees.Include(e => e.Post), "Id", "FirstName", employeeId);
            return View();
        }

      
        var employee = await _context.Employees.Include(e => e.Post).FirstOrDefaultAsync(e => e.Id == employeeId);
        if (employee == null)
        {
            ModelState.AddModelError("", "Выбранный сотрудник не найден.");
            ViewBag.EmployeeId = new SelectList(_context.Employees.Include(e => e.Post), "Id", "FirstName", employeeId);
            return View();
        }

      
        var user = new User
        {
            Username = username,
            PasswordHash = password, 
            EmployeeId = employee.Id,
            PostId = employee.PostId
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Login");
    }

    [HttpGet]
    [Authorize] 
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
    [HttpGet("account/profile/{id}")]
    public IActionResult Profile(int id)
    {
        var employee = _context.Employees
        .Include(e => e.Post)
        .Include(e => e.Orders)
        .FirstOrDefault(e => e.Id == id);

        if (employee == null)
        {
            return NotFound("Сотрудник не найден.");
        }

        var viewModel = new StoreSystem.ViewModels.EmployeeProfileViewModel
        {
            FullName = $"{employee.FirstName} {employee.LastName}",
            Post = employee.Post.Name,
            HireDate = employee.HireDate,
            Salary = employee.Salary,
            Bonus = employee.Bonus,
            SalesHistory = employee.Orders.Select(o => new OrderViewModel
            {
                OrderId = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount
            }).ToList()
        };

        // Возвращаем View с моделью
        return View(viewModel);
    }

}
