using Lab3.Auth.Services;

// 1. Инициализация строителя (Builder)
var builder = WebApplication.CreateBuilder(args);

// 2. Регистрация сервисов в контейнере (DI)
builder.Services.AddControllers();
builder.Services.AddSingleton<IAuthService, AuthService>();

// 3. Сборка приложения
var app = builder.Build();

// 4. Настройка конвейера запросов (Middleware)
app.UseHttpsRedirection();
app.MapControllers();

// Тестовые данные для эндпоинта
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// Определение маршрута (Minimal API)
app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

// 5. Запуск приложения (это должна быть последняя строка активного кода)
app.Run();

// --- СЕКЦИЯ ОБЪЯВЛЕНИЯ ТИПОВ (всегда в самом низу) ---

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Частичный класс нужен для доступа из тестов (интеграционное тестирование)
public partial class Program { }