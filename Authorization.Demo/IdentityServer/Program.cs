using IdentityServer;
using IdentityServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(config =>
{
    config.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    //config.UseInMemoryDatabase("Memory");
});

builder.Services.ConfigureApplicationCookie(configCookie =>
{
    configCookie.Cookie.Name = "IdentityServer.Cookie";
    configCookie.LoginPath = "/Auth/Login";
    configCookie.LogoutPath = "/Auth/Logout";
});

var assembly = typeof(Program).Assembly.GetName().Name;
builder.Services.AddIdentityServer()
    .AddAspNetIdentity<IdentityUser>()
    //.AddConfigurationStore(options =>
    //{
    //    options.ConfigureDbContext = b => b.UseSqlServer(
    //        builder.Configuration.GetConnectionString("DefaultConnection"),
    //         sql => sql.MigrationsAssembly(assembly));
    //})
    //.AddOperationalStore(options =>
    //{
    //    options.ConfigureDbContext = b => b.UseSqlServer(
    //    builder.Configuration.GetConnectionString("DefaultConnection"),
    //    sql => sql.MigrationsAssembly(assembly));
    //})
    .AddInMemoryApiResources(Configuration.GetApis())
    .AddInMemoryIdentityResources(Configuration.GetIdentityResources())
    .AddInMemoryClients(Configuration.GetClients())
    .AddDeveloperSigningCredential();

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    var user = new IdentityUser("Admin");
    userManager.CreateAsync(user, "Admin@123").GetAwaiter().GetResult();
    userManager.AddClaimAsync(user, new System.Security.Claims.Claim("rc.garndma", "big.cookie")).GetAwaiter().GetResult();
    userManager.AddClaimAsync(user, new System.Security.Claims.Claim("rc.api.garndma", "big.api.cookie")).GetAwaiter().GetResult();

    //scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>()
    //    .Database.Migrate();

    //var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

    //context.Database.Migrate();

    //if (!context.Clients.Any())
    //{
    //    foreach (var client in Configuration.GetClients())
    //    {
    //        context.Clients.Add(client.ToEntity());
    //    }
    //    context.SaveChanges();
    //}

    //if (!context.IdentityResources.Any())
    //{
    //    foreach (var resource in Configuration.GetIdentityResources())
    //    {
    //        context.IdentityResources.Add(resource.ToEntity());
    //    }
    //    context.SaveChanges();
    //}

    //if (!context.ApiResources.Any())
    //{
    //    foreach (var resource in Configuration.GetApis())
    //    {
    //        context.ApiResources.Add(resource.ToEntity());
    //    }
    //    context.SaveChanges();
    //}
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();
