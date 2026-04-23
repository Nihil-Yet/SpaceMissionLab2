using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceMission.Data
{
    [Table("PlanetaryMissions")]
    public class PlanetaryMissionEntity
    {
        [Key, ForeignKey("Mission")]
        public int Id { get; set; }

        public string Planet { get; set; } = string.Empty;
        public byte AtmoDensity { get; set; }
        public string LandingPointName { get; set; } = string.Empty;
        public short LandingPointX { get; set; }
        public short LandingPointY { get; set; }
        public short LandingPointR { get; set; }

        public MissionBase Mission { get; set; } = null!;
    }
}
