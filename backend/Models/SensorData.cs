using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string MAC { get; set; } = string.Empty;

        [Column("temperature")]
        public float Temperature { get; set; }

        [Column("humidity")]
        public float Humidity { get; set; }
    }
}
