using SensorApi.Models;
using SensorApi.Data;

namespace SensorApi.Services
{
    public class SensorService : ISensorService
    {
        private readonly SensorDbContext _context;

        public SensorService(SensorDbContext context)
        {
            _context = context;
        }

        public void ProcessData(SensorData data)
        {
            Console.WriteLine($"MAC: {data.MAC}, Temp: {data.Temperature}, Hum: {data.Humidity}, CO2: {data.CO2}");
            Console.WriteLine("Saving to DB...");

            _context.SensorData.Add(data);
            var result = _context.SaveChanges();

            Console.WriteLine($"Rows affected: {result}");
        }
    }
}
