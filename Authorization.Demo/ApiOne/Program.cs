var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", config =>
{
    config.Authority = "https://localhost:7223/";
    config.Audience = "ApiOne";
    config.RequireHttpsMetadata = false;
});

builder.Services.AddCors(confg =>
    confg.AddPolicy("AllowAll", 
        p => p.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()));

builder.Services.AddControllers();

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

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();
