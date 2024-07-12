using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using asp_login.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace asp_login.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [Authorize]
    // [Authorize(Roles = "Admin")]
    // [Authorize(Policy = "over18")]
    // [Authorize(Policy = "UsernamePolicy")]
    public IActionResult Index()
    {

        // Console.WriteLine("CookieAuthenticationDefaults.AuthenticationScheme: " + CookieAuthenticationDefaults.AuthenticationScheme);
        var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var lineIds = User.Claims.Where(c => c.Type == "lineId").Select(c => c.Value).ToList();
        ViewData["Role"] = roleClaim;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
