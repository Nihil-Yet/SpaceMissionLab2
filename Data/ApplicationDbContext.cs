using Microsoft.EntityFrameworkCore;

namespace SpaceMission.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<MissionEntity> Missions { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MissionEntity>()
                .HasDiscriminator<int>("MissionType")
                .HasValue<MissionEntity>((int)Models.MissionT.Orbital)
                .HasValue<MissionEntity>((int)Models.MissionT.Planetary);
        }
    }
}
