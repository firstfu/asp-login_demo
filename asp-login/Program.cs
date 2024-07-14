using asp_login.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


// 配置cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // cookie key
        options.Cookie.Name = "auth_cookie";
        // 登錄頁面
        options.LoginPath = "/Account/Login";
        // 登出頁面
        options.LogoutPath = "/Account/Logout";
    });

// 自定義授權規則-1
// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("UsernamePolicy", policy =>
//     {
//         policy.Requirements.Add(new UsernameRequirement("userDemo"));
//     });
// });
// builder.Services.AddSingleton<IAuthorizationHandler, UsernameAuthorizationHandler>();

// 自定義授權規則-2
// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("over18", policy =>
//     {
//         policy.RequireClaim("lineId", "18", "30");
//     });
// });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


// app.Use(async (context, next) =>
// {
//     await next();

//     Console.WriteLine(context.Response.StatusCode);

//     if (context.Response.StatusCode == 404)
//     {
//         context.Response.StatusCode = 200;
//         context.Response.ContentType = "application/json";
//         await context.Response.WriteAsync("{\"message\": \"Access Denied\", \"reason\": \"You do not have permission to access this resource.\"}");
//     }
// });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
