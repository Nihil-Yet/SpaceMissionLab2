using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceMission.Data
{
    [Table("OrbitalMissions")]
    public class OrbitalMissionEntity
    {
        [Key, ForeignKey("Mission")]
        public int Id { get; set; }

        public double CurrHeight { get; set; }
        public double TargetHeight { get; set; }
        public double Inclination { get; set; }
        public int EnergySource { get; set; }

        public MissionBase Mission { get; set; } = null!;
    }
}
