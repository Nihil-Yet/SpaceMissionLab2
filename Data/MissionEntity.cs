using System.ComponentModel.DataAnnotations;

namespace SpaceMission.Data
{
    public class MissionEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public int Budget { get; set; }
        public int Duration { get; set; }

        public int MissionType { get; set; }

        // Поля для OrbitalMission
        public double? CurrHeight { get; set; }
        public double? TargetHeight { get; set; }
        public double? Inclination { get; set; }
        public int? EnergySource { get; set; }

        // Поля для PlanetaryMission
        public string? Planet { get; set; }
        public byte? AtmoDensity { get; set; }
        public string? LandingPointName { get; set; }
        public short? LandingPointX { get; set; }
        public short? LandingPointY { get; set; }
        public short? LandingPointR { get; set; }
    }
}
