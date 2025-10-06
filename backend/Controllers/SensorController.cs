using Microsoft.AspNetCore.Mvc;
using SensorApi.Data;
using SensorApi.Models;

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


        // ✅ НОВЫЙ эндпоинт для получения последних данных
        [HttpGet("latest")]
        public IActionResult GetLatest()
        {
            try 
            {
                var latest = _db.SensorData
                    .OrderByDescending(d => d.Id)
                    .Select(d => new {
                        id = d.Id,
                        mac = d.MAC,  // ✅ возвращаем как "mac" (маленькими)
                        temp = d.Temperature,
                        hum = d.Humidity,
                        co2 = d.CO2
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
        
        Console.WriteLine($"Данные от {data.Name ?? data.MAC}: CO₂={data.CO2}, T={data.Temperature}°C, H={data.Humidity}%");
                
        return Ok(new { message = "Данные получены" });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
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

                foreach (var dev in devices)
                {
                    var secondsAgo = (DateTime.Now - dev.lastSeen).TotalSeconds;
                    Console.WriteLine($"  {dev.name}: {(dev.isOnline ? "ОНЛАЙН" : "ОФФЛАЙН")} (обновлялось {secondsAgo:F0}с назад)");
                }

                Console.WriteLine($"Найдено устройств: {devices.Count}");
                return Ok(devices);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения устройств: {ex.Message}");
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
                        co2 = d.CO2
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
                        MAC = d.MAC,
                        temp = d.Temperature,
                        hum = d.Humidity,
                        co2 = d.CO2
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