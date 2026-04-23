using System.ComponentModel.DataAnnotations;

namespace SpaceMission.Data
{
    public class MissionBase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public int Budget { get; set; }
        public int Duration { get; set; }

        public int MissionType { get; set; } // 1=Orbital, 2=Planetary

        public OrbitalMissionEntity? Orbital { get; set; }
        public PlanetaryMissionEntity? Planetary { get; set; }
    }
}
