using System.ComponentModel.DataAnnotations;

namespace SpaceMission.Data
{
    // Эта сущность хранится в БД
    public class MissionEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int Budget { get; set; }
        public int Duration { get; set; }

        // Дискриминатор: "Orbital" или "Planetary"
        public string MissionType { get; set; }

        // Поля для OrbitalMission
        public double? CurrHeight { get; set; }
        public double? TargetHeight { get; set; }
        public double? Inclination { get; set; }
        public string? EnergySource { get; set; }   // None, RTG, Solar

        // Поля для PlanetaryMission
        public string? Planet { get; set; }
        public byte? AtmoDensity { get; set; }
        public string? LandingPointName { get; set; }
        public short? LandingPointX { get; set; }
        public short? LandingPointY { get; set; }
        public short? LandingPointR { get; set; }
    }
}
