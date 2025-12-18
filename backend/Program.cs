using Microsoft.EntityFrameworkCore;
using SensorApi.Data;
using SensorApi.Services;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

var builder = WebApplication.CreateBuilder(args);

// 1. Настройка для PostgreSQL (Legacy Timestamp)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// 2. Подключение к базе данных
// Используем переменные окружения, если они заданы, иначе значения по умолчанию (для локальной разработки)
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection")
    ?? "Host=localhost;Port=5432;Database=postgres;Username=fek1r;Password=10021711"; 

builder.Services.AddDbContext<SensorDbContext>(options =>
    options.UseNpgsql(connectionString));

// 3. Регистрация сервисов
builder.Services.AddScoped<ISensorService, SensorService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. ✅ ИСПРАВЛЕНИЕ CORS (Разрешаем всё)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        // Разрешаем любой источник. В Production лучше использовать .WithOrigins()
        policy.AllowAnyOrigin()   
              .AllowAnyHeader()   
              .AllowAnyMethod();

    });
});

var app = builder.Build();

// 5. Автоматическое создание базы данных
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SensorDbContext>();
    try 
    {
        // Используем Migrate() вместо EnsureCreated().
        // Migrate() создает базу (если нет) и накатывает миграции.
        // Это безопасный аналог "CREATE TABLE IF NOT EXISTS" для всех таблиц.
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Логирование ошибки создания БД (если Postgres недоступен)
        Console.WriteLine($"Ошибка при создании БД: {ex.Message}");
    }
}

// 6. Настройка Pipeline 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ⛔️ ОТКЛЮЧЕНО для HTTP-доступа, как обсуждалось ранее
// app.UseHttpsRedirection();

// ✅ CORS включаем ДО авторизации и контроллеров
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

// 7. Логирование адресов при запуске
app.Lifetime.ApplicationStarted.Register(() =>
{
    var addresses = app.Urls;
    var allIPs = GetAllLocalIPAddresses();
    
    Console.WriteLine("\n" + new string('=', 60));
    Console.WriteLine("🚀 Сервер успешно запущен!");
    Console.WriteLine(new string('=', 60));
    
    foreach (var address in addresses)
    {
        var uri = new Uri(address);
        var port = uri.Port;
        var scheme = uri.Scheme;
        
        if (uri.Host == "0.0.0.0" || uri.Host == "[::]" || uri.Host == "::")
        {
            Console.WriteLine($"📡 Сервер слушает на всех интерфейсах (порт {port}):");
            Console.WriteLine($"   {scheme}://localhost:{port}");
            foreach (var ip in allIPs)
            {
                Console.WriteLine($"   {scheme}://{ip}:{port}");
            }
        }
        else
        {
            Console.WriteLine($"📡 {address}");
        }
    }
    
    Console.WriteLine(new string('=', 60));
    Console.WriteLine("📋 Доступные эндпоинты для проверки:");
    Console.WriteLine("   GET  /sensor/devices  - Список устройств (для React)");
    Console.WriteLine("   POST /sensor/receive  - Прием данных от ESP32");
    Console.WriteLine(new string('=', 60) + "\n");
});

app.Run();

// --- Вспомогательный метод для получения IP ---
static List<string> GetAllLocalIPAddresses()
{
    var ipAddresses = new List<string>();
    try
    {
        var interfaces = NetworkInterface.GetAllNetworkInterfaces()
            .Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
                        ni.NetworkInterfaceType != NetworkInterfaceType.Loopback);
        
        foreach (var ni in interfaces)
        {
            var ipProps = ni.GetIPProperties();
            foreach (var addr in ipProps.UnicastAddresses)
            {
                if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddresses.Add(addr.Address.ToString());
                }
            }
        }
    }
    catch
    {
        // Fallback
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddresses.Add(ip.ToString());
                }
            }
        }
        catch { }
    }
    return ipAddresses;
}