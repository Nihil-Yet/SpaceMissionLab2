using System;

namespace SpaceMission.Models
{
    public struct LandingPoint
    {
        public string Name;
        public short X;   // longitude, долгота
        public short Y;   // latitude, широта
        public short R;   // radius, радиус

        public LandingPoint(string name, short x, short y, short r)
        {
            Name = name;
            X = x;
            Y = y;
            R = r;
        }
    }

    public class PlanetaryMission : Mission
    {
        private string planet;
        private byte atmoDensity;   // 0 = none, 255 = very dense
        private LandingPoint landingPoint;

        public string Planet
        {
            get => planet;
            set => planet = value;
        }

        public byte AtmoDensity
        {
            get => atmoDensity;
            set => atmoDensity = value;
        }

        public LandingPoint LandingPoint
        {
            get => landingPoint;
            set => landingPoint = value;
        }

        // Constructors
        public PlanetaryMission()
            : base("None", 0, 0)
        {
            planet = "None";
            atmoDensity = 0;
            landingPoint = new LandingPoint("None", 0, 0, 0);
            MissionType = MissionT.Planetary;
        }

        public PlanetaryMission(string name,
                int budget, string planet)
            : base(name, budget, 365)
        {
            this.planet = planet;
            atmoDensity = 100;
            landingPoint = new LandingPoint("Default", 0, 0, 0);
            MissionType = MissionT.Planetary;
        }

        public PlanetaryMission(string name,
                int budget, int duration,
                string planet, byte atmoDensity,
                LandingPoint landingPoint)
            : base(name, budget, duration)
        {
            this.planet = planet;
            this.atmoDensity = atmoDensity;
            this.landingPoint = landingPoint;
            MissionType = MissionT.Planetary;
        }

        // Own methods
        public void SetLandingPoint(LandingPoint p)
        {
            landingPoint = p;
        }

        public bool HasAtmosphere()
        {
            return atmoDensity > 0;
        }

        public string ScientificGoal()
        {
            if (planet == "Mars")
                return "Поиск следов жизни и анализ геологии";
            if (planet == "Venus")
                return "Анализ климата и атмосферы";
            return "Планетарное исследование поверхности";
        }

        // Overrides
        public override double CalcFuelConsumption()
        {
            double baseFuel = 1000.0;
            if (HasAtmosphere())
            {
                double reduction = 0.4 * (atmoDensity / 255.0);
                baseFuel *= (1.0 - reduction);
            }
            return baseFuel;
        }

        public override float CalcRisk()
        {
            float risk = 0.3f;
            if (!HasAtmosphere())
                risk += 0.3f;
            else
                risk -= 0.1f * (atmoDensity / 255.0f);

            if (planet == "Venus")
                risk += 0.3f;
            else if (planet == "Mars")
                risk += 0.1f;

            return Math.Clamp(risk, 0.0f, 1.0f);
        }

        public override string GetInfo()
        {
            return $"{base.GetInfo()}\n" +
                   $"Mission type: Planetary\n" +
                   $"Planet: {planet}\n" +
                   $"Atmosphere: {(HasAtmosphere() ? "Yes" : "No")}\n" +
                   $"Landing point: {(string.IsNullOrEmpty(landingPoint.Name) ? "Unnamed" : landingPoint.Name)} " +
                   $"({landingPoint.X}, {landingPoint.Y}), R={landingPoint.R}";
        }
    }
}
