using asp_login.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;



public class AccountController : Controller
{

    // private static List<User> Users = new List<User>
    // {
    //     new User { Id = 1, Username = "test", Password = "password" }
    // };

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {

        // Console.WriteLine("CookieAuthenticationDefaults.AuthenticationScheme: " + CookieAuthenticationDefaults.AuthenticationScheme);

        if (!ModelState.IsValid)
        {
            // 如果模型狀態無效，返回登錄視圖並顯示錯誤信息
            return View(model);
        }

        if (model.Username != "test" || model.Password != "password")
        {
            ModelState.AddModelError(string.Empty, "用戶名或密碼錯誤。");
            return View(model);
        }

        if (model.Username == "test" && model.Password == "password")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "test"),
                new Claim(ClaimTypes.NameIdentifier, "001"),
                new Claim(ClaimTypes.Role, "Admin"),
                // 自定義聲明key-value
                new Claim("lineId", "14"),
                new Claim("lineId", "18"),
                new Claim("lineId", "30")
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
            return RedirectToAction("Index", "Home");
        }

        return View();

    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}