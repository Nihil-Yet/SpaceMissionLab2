using Microsoft.EntityFrameworkCore;

namespace SpaceMission.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<MissionBase> Missions { get; set; } = null!;
        public DbSet<OrbitalMissionEntity> OrbitalMissions { get; set; } = null!;
        public DbSet<PlanetaryMissionEntity> PlanetaryMissions { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MissionBase>()
                .HasOne(m => m.Orbital)
                .WithOne(o => o.Mission)
                .HasForeignKey<OrbitalMissionEntity>(o => o.Id);

            modelBuilder.Entity<MissionBase>()
                .HasOne(m => m.Planetary)
                .WithOne(p => p.Mission)
                .HasForeignKey<PlanetaryMissionEntity>(p => p.Id);
        }
    }
}
