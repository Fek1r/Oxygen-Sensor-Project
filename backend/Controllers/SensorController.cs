using Microsoft.AspNetCore.Mvc;
using SensorApi.Data;
using SensorApi.Models;
using System.Text;

namespace SensorApi.Controllers
{
    [ApiController]
    [Route("sensor")]
    public class SensorController : ControllerBase
    {
        private readonly SensorDbContext _db;

        public SensorController(SensorDbContext db)
        {
            _db = db;
        }

        // ✅ Получение данных за период
        [HttpGet("history")]
        public IActionResult GetHistory([FromQuery] string mac, [FromQuery] string period = "live")
        {
            try 
            {
                DateTime startTime;
                
                switch (period.ToLower())
                {
                    case "hour":
                        startTime = DateTime.Now.AddHours(-1);
                        break;
                    case "day":
                        startTime = DateTime.Now.AddDays(-1);
                        break;
                    case "week":
                        startTime = DateTime.Now.AddDays(-7);
                        break;
                    case "live":
                    default:
                        startTime = DateTime.Now.AddMinutes(-30); // Последние 30 минут для live
                        break;
                }

                var query = _db.SensorData
                    .Where(d => d.LastSeen >= startTime)
                    .OrderBy(d => d.LastSeen);

                // Если указан MAC, фильтруем по устройству
                if (!string.IsNullOrEmpty(mac))
                {
                    query = (IOrderedQueryable<SensorData>)query.Where(d => d.MAC == mac);
                }

                var data = query
                    .Select(d => new {
                        id = d.Id,
                        mac = d.MAC,
                        name = d.Name,
                        temp = d.Temperature,
                        hum = d.Humidity,
                        co2 = d.CO2,
                        timestamp = d.LastSeen
                    })
                    .ToList();

                Console.WriteLine($"📊 Запрос данных: период={period}, MAC={mac ?? "все"}, найдено={data.Count}");
                
                return Ok(new { 
                    period = period,
                    startTime = startTime,
                    count = data.Count,
                    data = data 
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка получения истории: {ex.Message}");
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        // ✅ Экспорт данных в CSV
        [HttpGet("export/csv")]
        public IActionResult ExportToCsv([FromQuery] string mac, [FromQuery] string period = "day")
        {
            try 
            {
                DateTime startTime;
                
                switch (period.ToLower())
                {
                    case "hour":
                        startTime = DateTime.Now.AddHours(-1);
                        break;
                    case "week":
                        startTime = DateTime.Now.AddDays(-7);
                        break;
                    case "all":
                        startTime = DateTime.MinValue;
                        break;
                    case "day":
                    default:
                        startTime = DateTime.Now.AddDays(-1);
                        break;
                }

                var query = _db.SensorData
                    .Where(d => d.LastSeen >= startTime)
                    .OrderBy(d => d.LastSeen);

                if (!string.IsNullOrEmpty(mac))
                {
                    query = (IOrderedQueryable<SensorData>)query.Where(d => d.MAC == mac);
                }

                var data = query.ToList();

                if (!data.Any())
                {
                    return NotFound("Нет данных для экспорта");
                }

                // Формируем CSV
                var csv = new StringBuilder();
                csv.AppendLine("Timestamp,MAC,Device Name,Temperature (°C),Humidity (%),CO2 (ppm)");
                
                foreach (var row in data)
                {
                    csv.AppendLine($"{row.LastSeen:yyyy-MM-dd HH:mm:ss},{row.MAC},{row.Name ?? "N/A"},{row.Temperature},{row.Humidity},{row.CO2}");
                }

                var fileName = $"sensor_data_{(string.IsNullOrEmpty(mac) ? "all" : mac)}_{period}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var bytes = Encoding.UTF8.GetBytes(csv.ToString());
                
                Console.WriteLine($"📥 Экспорт CSV: {data.Count} записей, файл={fileName}");
                
                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка экспорта: {ex.Message}");
                return StatusCode(500, $"Ошибка экспорта: {ex.Message}");
            }
        }

        // ✅ Получение последних данных
        [HttpGet("latest")]
        public IActionResult GetLatest()
        {
            try 
            {
                var latest = _db.SensorData
                    .OrderByDescending(d => d.Id)
                    .Select(d => new {
                        id = d.Id,
                        mac = d.MAC,
                        name = d.Name,
                        temp = d.Temperature,
                        hum = d.Humidity,
                        co2 = d.CO2,
                        lastSeen = d.LastSeen
                    })
                    .FirstOrDefault();

                if (latest == null)
                    return NotFound("Нет данных в базе");

                return Ok(latest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка получения данных: {ex.Message}");
            }
        }

        [HttpPost("receive")]
        public IActionResult Receive([FromBody] SensorData data)
        {
            if (data == null)
                return BadRequest("Нет данных от сенсора");

            try 
            {
                data.LastSeen = DateTime.Now;
                _db.SensorData.Add(data);
                _db.SaveChanges();
                
                Console.WriteLine($"✅ Данные от {data.Name ?? data.MAC}: CO₂={data.CO2}, T={data.Temperature}°C, H={data.Humidity}%");
                
                return Ok(new { message = "Данные получены" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка: {ex.Message}");
                return StatusCode(500, $"Ошибка сохранения: {ex.Message}");
            }
        }

        [HttpGet("devices")]
        public IActionResult GetDevices()
        {
            try
            {
                var allData = _db.SensorData
                    .OrderByDescending(d => d.Id)
                    .ToList();

                var devices = allData
                    .GroupBy(d => d.MAC)
                    .Select(g => g.First())
                    .Select(d => new
                    {
                        mac = d.MAC,
                        name = string.IsNullOrEmpty(d.Name)
                            ? $"ESP32 {d.MAC.Substring(Math.Max(0, d.MAC.Length - 5))}"
                            : d.Name,
                        temp = d.Temperature,
                        hum = d.Humidity,
                        co2 = d.CO2,
                        lastSeen = d.LastSeen,
                        isOnline = (DateTime.Now - d.LastSeen).TotalSeconds < 45
                    })
                    .ToList();

                Console.WriteLine($"📡 Найдено устройств: {devices.Count}");
                return Ok(devices);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка получения устройств: {ex.Message}");
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpGet("device/{mac}")]
        public IActionResult GetDeviceData(string mac)
        {
            try 
            {
                var latest = _db.SensorData
                    .Where(d => d.MAC == mac)
                    .OrderByDescending(d => d.Id)
                    .Select(d => new {
                        id = d.Id,
                        mac = d.MAC,
                        name = d.Name,
                        temp = d.Temperature,
                        hum = d.Humidity,
                        co2 = d.CO2,
                        lastSeen = d.LastSeen
                    })
                    .FirstOrDefault();

                if (latest == null)
                    return NotFound($"Устройство {mac} не найдено");

                return Ok(latest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка: {ex.Message}");
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            try 
            {
                var all = _db.SensorData
                    .OrderByDescending(d => d.Id)
                    .Take(50)
                    .Select(d => new {
                        id = d.Id,
                        mac = d.MAC,
                        name = d.Name,
                        temp = d.Temperature,
                        hum = d.Humidity,
                        co2 = d.CO2,
                        lastSeen = d.LastSeen
                    })
                    .ToList();

                return Ok(all);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка получения данных: {ex.Message}");
            }
        }
    }
}