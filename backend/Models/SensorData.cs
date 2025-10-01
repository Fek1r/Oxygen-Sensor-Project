using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SensorApi.Models
{
    [Table("sensors")]
    public class SensorData
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("mac")]
        [JsonPropertyName("MAC")]
        public string MAC { get; set; } = string.Empty;

        [Column("temperature")]
        [JsonPropertyName("temp")]
        public float Temperature { get; set; }

        [Column("humidity")]
        [JsonPropertyName("hum")]
        public float Humidity { get; set; }

        [Column("co2")]
        [JsonPropertyName("co2")]
        public float CO2 { get; set; }  // ✅ новое поле
    }
}
