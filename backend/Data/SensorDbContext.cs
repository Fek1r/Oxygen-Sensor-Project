
using Microsoft.EntityFrameworkCore;
using SensorApi.Models;

namespace SensorApi.Data
{
    public class SensorDbContext : DbContext
    {
        public SensorDbContext(DbContextOptions<SensorDbContext> options) : base(options) { }

        public DbSet<SensorData> SensorReadings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SensorData>().ToTable("sensors");
        }
    }
}


// CREATE TABLE sensors (
//     Id SERIAL PRIMARY KEY,
//     MAC VARCHAR(17) NOT NULL,
//     Temperature REAL NOT NULL,
//     Humidity REAL NOT NULL
// );
