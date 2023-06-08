using Basics.AuthorizationRequirements;
using Basics.Controllers;
using Basics.CustomPolicyProvider;
using Basics.Transformer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", configureOptions =>
{
    configureOptions.Cookie.Name = "Grandmas.Cookie";
    configureOptions.LoginPath = "/Home/Authenticate";
});

builder.Services.AddAuthorization(config =>
{
    //var defaultAuthBuilder = new AuthorizationPolicyBuilder();
    //var defaultPolicy = defaultAuthBuilder.RequireAuthenticatedUser()
    //    .RequireClaim(ClaimTypes.DateOfBirth)
    //    .Build();
    //config.DefaultPolicy = defaultPolicy;

    //config.AddPolicy("Claim.DoB", policyBuilder =>
    //{
    //    policyBuilder.RequireClaim(ClaimTypes.DateOfBirth);
    //});

    config.AddPolicy("Admin", policyBuilder =>
    {
        policyBuilder.RequireClaim(ClaimTypes.Role, "Admin");
    });

    config.AddPolicy("Claim.DoB", policyBuilder =>
    {
        policyBuilder.RequireCustomClaim(ClaimTypes.DateOfBirth);
    });
});

builder.Services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, SecurityLevelHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CookieJarAuthorizationHandle>();
builder.Services.AddScoped<IClaimsTransformation, ClaimsTransformation>();

builder.Services.AddControllersWithViews(config =>
{
    var defaultAuthBuilder = new AuthorizationPolicyBuilder();
    var defaultAuthPolicy = defaultAuthBuilder.RequireAuthenticatedUser().Build();

    //// global authorization filter
    //config.Filters.Add(new AuthorizeFilter(defaultAuthPolicy));
});

builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
    options.Conventions.AuthorizePage("/Razor/Secured");
    options.Conventions.AuthorizePage("/Razor/Policy", "Admin");
    options.Conventions.AuthorizeFolder("/RazorSecured");
    options.Conventions.AllowAnonymousToPage("/RazorSecured/Anon");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Who are you?
app.UseAuthentication();

// are you allowed?
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    endpoints.MapRazorPages();
});

app.Run();
