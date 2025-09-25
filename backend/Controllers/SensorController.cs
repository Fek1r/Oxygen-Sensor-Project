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

        [HttpPost("receive")]
        public IActionResult Receive([FromBody] SensorData data)
        {
            if (data == null)
                return BadRequest("Нет данных от сенсора");

            try 
            {
                _db.SensorData.Add(data);
                _db.SaveChanges();
                
                return Ok(new { 
                    message = "Данные получены", 
                    data = new {
                        id = data.Id,
                        mac = data.MAC,
                        temp = data.Temperature,  // используем правильные имена
                        hum = data.Humidity
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка сохранения: {ex.Message}");
            }
        }

        [HttpGet("latest")]
        public IActionResult GetLatest()
        {
            try 
            {
                Console.WriteLine("🔍 Запрос на получение последних данных...");
                
                var latest = _db.SensorData
                    .OrderByDescending(d => d.Id)
                    .FirstOrDefault();

                if (latest == null)
                {
                    Console.WriteLine("❌ Данных в базе нет");
                    return NotFound("Данных пока нет");
                }

                Console.WriteLine($"✅ Найдены данные: ID={latest.Id}, MAC={latest.MAC}, Temp={latest.Temperature}, Hum={latest.Humidity}");
                return Ok(latest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Ошибка: {ex.Message}");
                return StatusCode(500, $"Ошибка получения данных: {ex.Message}");
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
                        temp = d.Temperature,
                        hum = d.Humidity
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