using Microsoft.EntityFrameworkCore;
using SensorApi.Models;

namespace SensorApi.Data
{
    public class SensorDbContext : DbContext
    {
        public SensorDbContext(DbContextOptions<SensorDbContext> options)
            : base(options)
        {
        }

        public DbSet<SensorData> SensorData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<SensorData>(entity =>
            {
                entity.ToTable("sensors");
                entity.HasKey(e => e.Id);
                
                // ✅ Исправляем названия колонок, чтобы совпадали с JsonPropertyName
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.MAC).HasColumnName("MAC");           // было "mac"
                entity.Property(e => e.Temperature).HasColumnName("temp");   // было "temperature" 
                entity.Property(e => e.Humidity).HasColumnName("hum");      // было "humidity"
            });
        }
    }
}



// CREATE TABLE sensors (
//     Id SERIAL PRIMARY KEY,
//     MAC VARCHAR(17) NOT NULL,
//     Temperature REAL NOT NULL,
//     Humidity REAL NOT NULL
// );
