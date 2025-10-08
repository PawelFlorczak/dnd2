using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using DiceAPI.Data;
using DiceAPI.Hubs;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddControllers();
builder.Services.AddSignalR();

// 🧩 Wybór bazy w zależności od środowiska
var env = builder.Environment.EnvironmentName;
if (env == "Development")
{
    builder.Services.AddDbContext<DiceContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
}
else
{
    builder.Services.AddDbContext<DiceContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowAll");

app.MapControllers();
app.MapHub<DiceHub>("/DiceHub");

app.Run();