using System.Collections.Generic;

namespace SpaceMission
{
    public interface IPlanetaryData
    {
        double RadiusKm { get; }
        double GravitationalParameter { get; }   // м³/с²
        double RotationPeriodSeconds { get; }     // сидерический период вращения, секунды
        List<(double minHeight, double maxHeight, double riskFactor)> DebrisRiskZones { get; }
    }
}
