using asp_login.Dtos;
using asp_login.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;



[Route("[controller]")]
public class AccountController : Controller
{

    // private static List<User> Users = new List<User>
    // {
    //     new User { Id = 1, Username = "test", Password = "password" }
    // };

    private readonly ILogger<AccountController> _logger;
    private readonly IConfiguration _configuration;


    public AccountController(IConfiguration configuration, ILogger<AccountController> logger)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [Route("Login", Name = "Login")]
    [HttpGet]
    public IActionResult Login()
    {

        // 從 TempData 讀取錯誤消息並添加到 ModelState
        if (TempData["lineLoginError"] != null)
        {
            string errorMessage = TempData["lineLoginError"]?.ToString() ?? "發生未知錯誤";
            ModelState.AddModelError(string.Empty, errorMessage);
        }
        return View();
    }

    [Route("Login", Name = "Login")]
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // 如果模型狀態無效，返回登錄視圖並顯示錯誤信息
            return View(model);
        }

        if (model.Username != "test" || model.Password != "123456")
        {
            ModelState.AddModelError(string.Empty, "用戶名或密碼錯誤。");
            return View(model);
        }

        if (model.Username == "test" && model.Password == "123456")
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


    // line登入
    // public async Task<IActionResult> LoginWithLine(string code)
    [Route("LoginWithLine", Name = "LoginWithLine")]
    [HttpGet]
    public ActionResult LoginWithLine(string code)
    {
        // 1. 用code取得access_token
        // 2. 用access_token取得用戶資料
        // 3. 用用戶資料登入
        var lineLoginUrl = $"https://access.line.me/oauth2/v2.1/authorize?response_type=code&client_id={_configuration["Line:ChannelId"]}&redirect_uri={_configuration["Line:CallbackUrl"]}&state=12345abcde&scope=profile%20openid%20email";
        return Redirect(lineLoginUrl);
    }



    [Route("line-login-callback", Name = "LineLoginCallback")]
    [HttpGet]
    public async Task<ActionResult<dynamic>> LineLoginCallback([FromQuery] string code, string state)
    {

        if (string.IsNullOrEmpty(code))
        {
            // 跳轉回登入頁面
            TempData["lineLoginError"] = "Line登入失敗";
            return RedirectToAction("Login", "Account");
        }

        LineToken? lineToken = await _getLineLoginToken(code);

        if (lineToken == null)
        {
            // 跳轉回登入頁面
            TempData["lineLoginError"] = "Line登入失敗";
            return RedirectToAction("Login", "Account");
        }


        // 取得line用戶登入資訊
        dynamic? lineProfile = await _getLineProfile(lineToken.id_token);
        if (lineProfile == null)
        {
            // 跳轉回登入頁面
            TempData["lineLoginError"] = "Line登入失敗";
            return RedirectToAction("Login", "Account");
        }


        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, lineProfile?.name ?? ""),
            new Claim(ClaimTypes.NameIdentifier, ""),
            new Claim(ClaimTypes.Email, lineProfile?.email ?? ""),
            // new Claim("picture", lineProfile?.picture ?? ""),
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true
        };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        return RedirectToAction("Index", "Home");


    }

    // 取得line token
    public async Task<LineToken?> _getLineLoginToken(string code)
    {
        // 1. 用code取得access_token
        // 2. 用access_token取得用戶資料
        var url = "https://api.line.me/oauth2/v2.1/token";
        var body = new Dictionary<string, string>{
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", _configuration["Line:CallbackUrl"]! },
            { "client_id", _configuration["Line:ChannelId"]! },
            { "client_secret", _configuration["Line:ChannelSecret"]! }
        };
        using HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(body));
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        LineToken _content = JsonConvert.DeserializeObject<LineToken>(content)!;
        return _content;
    }

    // 取得line用戶資料
    public async Task<dynamic?> _getLineProfile(string id_token)
    {

        var url = "https://api.line.me/oauth2/v2.1/verify";

        var body = new Dictionary<string, string>
        {
            { "id_token", id_token },
            { "client_id", _configuration["Line:ChannelId"]! }
        };

        using HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(body));
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        var bodyParam = new
        {
            iss = "",
            sub = "",
            aud = "",
            exp = "",
            iat = "",
            // amr = new string[] { },
            name = "",
            picture = "",
            email = "",
        };
        bodyParam = JsonConvert.DeserializeAnonymousType(content, bodyParam);
        return bodyParam;

    }




    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

}