using System.Text.Json.Serialization;

namespace SensorApi.Models;

public class SensorData
{
    [JsonPropertyName("MAC")]
    public string MAC { get; set; }

    [JsonPropertyName("temp")]
    public float Temperature { get; set; }

    [JsonPropertyName("hum")]
    public float Humidity { get; set; }
}
