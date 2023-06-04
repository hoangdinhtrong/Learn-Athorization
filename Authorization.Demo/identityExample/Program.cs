using IdentityExample.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(config =>
{
    config.UseInMemoryDatabase("Memory");
});

// Identity Registers the services
builder.Services.AddIdentity<IdentityUser, IdentityRole>(config =>
    {
        config.Password.RequiredLength = 4;
        config.Password.RequireDigit = false;
        config.Password.RequireNonAlphanumeric = false;
        config.Password.RequireUppercase = false;
        config.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(configCookie =>
{
    configCookie.Cookie.Name = "Identity.Cookie";
    configCookie.LoginPath = "/Home/Login";
    configCookie.LogoutPath = "/Home/Logout";
});

builder.Services.AddMailKit(mailConfig => mailConfig.UseMailKit(builder.Configuration.GetSection("Email").Get<MailKitOptions>()));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Who are you?
app.UseAuthentication();

app.UseRouting();

// are you allowed?
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();
