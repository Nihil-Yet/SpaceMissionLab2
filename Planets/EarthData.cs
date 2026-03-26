using System.Collections.Generic;

namespace SpaceMission
{
    public class EarthData : IPlanetaryData
    {
        public double RadiusKm => 6371.0;
        public double GravitationalParameter => 3.986004418e14;
        public double RotationPeriodSeconds => 86164.0;   // 23h 56m 4s

        private static readonly List<(double, double, double)> _debrisZones = new()
        {
            (0, 500, 1.8),
            (500, 1000, 1.5),
            (1000, 2000, 1.2),
            (2000, 5000, 1.0),
            (5000, double.MaxValue, 0.8)
        };

        public List<(double minHeight, double maxHeight, double riskFactor)> DebrisRiskZones => _debrisZones;
    }
}
