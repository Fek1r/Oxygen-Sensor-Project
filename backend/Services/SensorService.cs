using SensorApi.Models;
using System;

namespace SensorApi.Services
{
    public class SensorService : ISensorService
    {
        public void ProcessData(SensorData data)
        {
            Console.WriteLine($"MAC: {data.MAC}, Temp: {data.Temperature}, Hum: {data.Humidity}");
        }
    }
}
