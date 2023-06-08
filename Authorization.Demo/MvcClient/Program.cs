using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(config =>
{
    config.DefaultScheme = "Cookie";
    config.DefaultChallengeScheme = "oidc";
}).AddCookie("Cookie")
.AddOpenIdConnect("oidc", config =>
{
    config.ClientId = "client_id_mvc";
    config.ClientSecret = "client_secret_mvc";
    config.SaveTokens = true;
    config.Authority = "https://localhost:7028/";
    config.ResponseType = OpenIdConnectResponseType.Code;
    config.SignedOutCallbackPath = "/Home/Index";
    config.UsePkce = true;

    // configuration cookie claim mapping
    config.ClaimActions.DeleteClaim("arm");
    config.ClaimActions.DeleteClaim("s_hash");
    config.ClaimActions.MapUniqueJsonKey("MVCClient.Grandma", "rc.garndma");

    // two trips to load claims in the cookie
    // but the id token is smaller!
    config.GetClaimsFromUserInfoEndpoint = true;
    // configuration scope
    config.Scope.Clear();
    config.Scope.Add("openid");
    config.Scope.Add("ApiOne");
    config.Scope.Add("ApiTwo");
    config.Scope.Add("rc.scope");
    config.Scope.Add("offline_access");
});

builder.Services.AddHttpClient();

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

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();
