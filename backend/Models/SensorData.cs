using System;
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
        [Column("MAC")]
        [JsonPropertyName("MAC")]
        public string MAC { get; set; } = string.Empty;

        [Column("temp")]
        [JsonPropertyName("temp")]
        public float Temperature { get; set; }

        [Column("hum")]
        [JsonPropertyName("hum")]
        public float Humidity { get; set; }

        [Column("co2")]
        [JsonPropertyName("co2")]
        public float CO2 { get; set; }

        [Column("name")]
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [Column("last_seen")]
        [JsonPropertyName("lastSeen")]
        public DateTime LastSeen { get; set; } = DateTime.Now;
    }
}