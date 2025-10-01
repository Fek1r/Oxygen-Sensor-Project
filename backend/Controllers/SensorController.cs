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
                        temp = data.Temperature,
                        hum = data.Humidity,
                        co2 = data.CO2
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка сохранения: {ex.Message}");
            }
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