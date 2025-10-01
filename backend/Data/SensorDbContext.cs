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
                entity.Property(e => e.Humidity).HasColumnName("hum"); 
                entity.Property(e => e.CO2).HasColumnName("co2");      // было "humidity"
            });
        }
    }
}

                // CREATE TABLE sensors (
                //     id SERIAL PRIMARY KEY,
                //     "MAC" VARCHAR(17) NOT NULL,  -- адрес устройства
                //     temp REAL NOT NULL,          -- температура (°C)
                //     hum REAL NOT NULL,           -- влажность (%)
                //     co2 REAL NOT NULL            -- CO₂ (ppm)
                // );



