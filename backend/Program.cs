using Microsoft.EntityFrameworkCore;
using SensorApi.Data;
using SensorApi.Services;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

var builder = WebApplication.CreateBuilder(args);

// строка подключения к PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection")
    ?? "Host=localhost;Port=5433;Database=postgres;Username=postgres;Password=10021711";

builder.Services.AddDbContext<SensorDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ISensorService, SensorService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Разрешаем CORS для фронтенда
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Автоматическое создание базы
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SensorDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ Подключаем CORS ДО контроллеров
app.UseCors("AllowFrontend");

app.UseAuthorization();
app.MapControllers();

// ✅ Логирование адресов при запуске
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
        
        // Если слушаем на 0.0.0.0 или ::, показываем все доступные IP
        if (uri.Host == "0.0.0.0" || uri.Host == "::")
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
    Console.WriteLine("📋 Доступные эндпоинты:");
    Console.WriteLine("   POST /sensor/receive  - Прием данных от ESP32");
    Console.WriteLine("   GET  /sensor/latest   - Последние данные");
    Console.WriteLine("   GET  /sensor/all      - Все данные (последние 50)");
    Console.WriteLine(new string('=', 60) + "\n");
});

app.Run();

// Получить все локальные IP-адреса
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
        // Fallback метод
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