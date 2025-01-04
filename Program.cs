using AspNetCoreHero.ToastNotification;
using ecommerce_final;
using ecommerce_final.Entities;
using ecommerce_final.Extensions;
using ecommerce_final.Models;
using ecommerce_final.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddDbContext<EcommerceFinalContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add ToastNotification service (Chỉ cần thêm một lần)
builder.Services.AddNotyf(config => {
    config.DurationInSeconds = 10;
    config.IsDismissable = true;
    config.Position = NotyfPosition.BottomRight;

});

builder.Services.AddScoped<EmailService>();

builder.Services.AddScoped<OtpService>();

builder.Services.AddScoped<OrderService>();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Cấu hình MailChimp từ appsettings
builder.Services.Configure<MailChimpSettings>(builder.Configuration.GetSection("MailChimp"));

// Đảm bảo đăng ký MailChimpService với DI container
builder.Services.AddSingleton<MailChimpService>();


//Config Authen
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        options.SlidingExpiration = true;
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Shared/404Page";
    });


builder.Services.AddSingleton(x =>
    new PaypalClient(
        builder.Configuration["PayPalOptions:ClientId"],
        builder.Configuration["PayPalOptions:ClientSecret"],
        builder.Configuration["PayPalOptions:Mode"]
    )
);





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

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "productDetail",
    pattern: "Product/Detail/{id}/{name?}",  // Định nghĩa route thân thiện với SEO
    defaults: new { controller = "Product", action = "Detail" }
);


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

app.Run();
