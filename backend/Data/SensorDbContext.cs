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

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.MAC).HasColumnName("MAC");
                entity.Property(e => e.Temperature).HasColumnName("temp");
                entity.Property(e => e.Humidity).HasColumnName("hum");
                entity.Property(e => e.CO2).HasColumnName("co2");
                entity.Property(e => e.Name).HasColumnName("name");
                // ✅ ДОБАВЬТЕ ЭТУ СТРОКУ
                entity.Property(e => e.LastSeen).HasColumnName("last_seen");
            });
        }
    }
}