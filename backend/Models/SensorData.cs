using System.Text.Json.Serialization;

namespace SensorApi.Models
{
    public class SensorData
    {
        [JsonPropertyName("MAC")]
        public required string MAC { get; set; }

        [JsonPropertyName("Temperature")]
        public required float Temperature { get; set; }

        [JsonPropertyName("Humidity")]
        public required float Humidity { get; set; }
    }
}
