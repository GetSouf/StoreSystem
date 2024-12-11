using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StoreSystem.Models;
using testproject.Models;
using Microsoft.EntityFrameworkCore;

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
            new Claim(ClaimTypes.Role, user.Post.Name)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
        var authProperties = new AuthenticationProperties { };

        await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Login", "Account");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
