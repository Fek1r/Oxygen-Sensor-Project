using Microsoft.EntityFrameworkCore;
using SensorApi.Data;
using SensorApi.Services;

var builder = WebApplication.CreateBuilder(args);

// строка подключения к PostgreSQL (замени своими параметрами)
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection")
    ?? "Host=localhost;Port=5433;Database=postgres;Username=postgres;Password=10021711";

// Регистрируем DbContext
builder.Services.AddDbContext<SensorDbContext>(options =>
    options.UseNpgsql(connectionString));

// Регистрируем сервис
builder.Services.AddScoped<ISensorService, SensorService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Автоматическая проверка и создание базы
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SensorDbContext>();
    db.Database.EnsureCreated();  // если базы нет - создаст
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
