using Microsoft.EntityFrameworkCore;

namespace SpaceMission.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<MissionEntity> Missions { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Таблица будет одна, но с разными полями
            modelBuilder.Entity<MissionEntity>()
                .HasDiscriminator<string>("MissionType")
                .HasValue<MissionEntity>("Orbital")
                .HasValue<MissionEntity>("Planetary");
        }
    }
}
