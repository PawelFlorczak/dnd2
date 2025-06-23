using Microsoft.EntityFrameworkCore;
using DiceAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Pobierz connection string z appsettings.json lub zmiennej środowiskowej DATABASE_URL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? Environment.GetEnvironmentVariable("DATABASE_URL");

// UWAGA: jeśli connection string jest w formacie postgres://... (Render tak daje),
// musimy przekonwertować go na format zrozumiały dla Npgsql/EF Core
if (connectionString.StartsWith("postgres://"))
{
    var uri = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':');

    connectionString =
        $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};" +
        $"Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
    
    Console.WriteLine("ENV DATABASE_URL: " + Environment.GetEnvironmentVariable("DATABASE_URL"));
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Brak ustawionej zmiennej DATABASE_URL ani wpisu DefaultConnection w appsettings.");
    }
}

builder.Services.AddDbContext<DiceContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();