using SensorApi.Models;

namespace SensorApi.Services
{
    public interface ISensorService
    {
        void ProcessData(SensorData data);
    }
}
