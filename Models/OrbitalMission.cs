using System;

namespace SpaceMission.Models
{
    public enum EnergySource
    {
        None,
        RTG,
        Solar
    }

    public enum OrbitType
    {
        LowEarth,
        Polar,
        Geostationary,
        Unknown
    }

    public struct OrbitState
    {
        public double CurrHeight;      // km
        public OrbitType Type;
        public double Inclination;     // degrees
        public double Period;          // minutes
    }

    public class OrbitalMission : Mission
    {
        private double currHeight;          // текущая высота, км
        private double targetHeight;        // целевая высота для манёвра, км
        private double inclination;         // наклонение орбиты, градусы
        private EnergySource energySource;
        private IPlanetaryData planetaryData;

        // геттеры/сеттеры
        public double CurrHeight
        {
            get => currHeight;
            set => currHeight = value;
        }

        public double TargetHeight
        {
            get => targetHeight;
            set => targetHeight = value;
        }

        public double Inclination
        {
            get => inclination;
            set => inclination = value;
        }

        public EnergySource EnergySource
        {
            get => energySource;
            set => energySource = value;
        }

        public IPlanetaryData PlanetaryData
        {
            get => planetaryData;
            set => planetaryData = value;
        }

        // Конструкторы
        public OrbitalMission() : base("None", 0, 0)
        {
            currHeight = 0.0;
            targetHeight = 0.0;
            inclination = 0.0;
            energySource = EnergySource.None;
            planetaryData = new EarthData();
            MissionType = MissionT.Orbital;
        }

        public OrbitalMission(string name, int budget) : base(name, budget, 180)
        {
            currHeight = 500.0;
            targetHeight = 500.0;
            inclination = 0.0;
            energySource = EnergySource.Solar;
            planetaryData = new EarthData();
            MissionType = MissionT.Orbital;
        }

        public OrbitalMission(string name, int budget, int duration,
                              double currHeight, double targetHeight, double inclination,
                              EnergySource energySource, IPlanetaryData planetaryData)
            : base(name, budget, duration)
        {
            this.currHeight = currHeight;
            this.targetHeight = targetHeight;
            this.inclination = inclination;
            this.energySource = energySource;
            this.planetaryData = planetaryData;
            MissionType = MissionT.Orbital;
        }

        // Изменение текущей высоты (например, для манёвра)
        public double ChangeOrbit(double delta)
        {
            currHeight += delta;
            return currHeight;
        }

        // Установка целевой высоты для расчёта топлива
        public void SetTargetHeight(double newTarget)
        {
            targetHeight = newTarget;
        }

        // Получение состояния орбиты с расчётом периода и типа
        public OrbitState OrbitState()
        {
            // Расчёт периода круговой орбиты по формуле T = 2π √(a³/(GM))
            double rMeters = (planetaryData.RadiusKm + currHeight) * 1000.0;
            double periodSeconds = 2.0 * Math.PI * Math.Sqrt(rMeters * rMeters * rMeters / 
                    planetaryData.GravitationalParameter);
            double periodMinutes = periodSeconds / 60.0;

            // Определение типа орбиты
            OrbitType type = OrbitType.Unknown;
            const double inclinationEps = 10.0;   // допуск для наклонения, градусы
            const double periodEps = 0.001;       // допуск для периода (0.1%)

            // Геостационарная: период близок к периоду вращения планеты и наклонение близко к 0
            if (Math.Abs(periodSeconds -
                    planetaryData.RotationPeriodSeconds) /
                    planetaryData.RotationPeriodSeconds < periodEps &&
                    Math.Abs(inclination) < inclinationEps)
            {
                type = OrbitType.Geostationary;
            }
            // Полярная: наклонение близко к 90°
            else if (Math.Abs(inclination - 90.0) < inclinationEps)
            {
                type = OrbitType.Polar;
            }
            // Низкая околоземная (условно до 2000 км)
            else if (currHeight < 2000)
            {
                type = OrbitType.LowEarth;
            }
            // Остальное – Unknown

            return new OrbitState
            {
                CurrHeight = currHeight,
                Type = type,
                Inclination = inclination,
                Period = periodMinutes
            };
        }

        // Коэффициент риска от космического мусора
        private double GetDebrisRiskFactor()
        {
            foreach (var zone in planetaryData.DebrisRiskZones)
            {
                if (currHeight >= zone.minHeight && currHeight < zone.maxHeight)
                    return zone.riskFactor;
            }
            return 1.0; // по умолчанию
        }

        // Расход топлива: зависит от разницы высот и гравитационного параметра планеты
        public override double CalcFuelConsumption()
        {
            // Простая модель: расход пропорционален изменению высоты и гравитационному ускорению на средней высоте
            double deltaHeightKm = Math.Abs(targetHeight - currHeight);
            if (deltaHeightKm < 1e-6) return 0.0;

            // Средний радиус в метрах
            double avgRadiusM = (planetaryData.RadiusKm + (currHeight + targetHeight) / 2.0) * 1000.0;
            // Ускорение свободного падения на средней высоте: g = GM / r²
            double g = planetaryData.GravitationalParameter / (avgRadiusM * avgRadiusM);
            // Коэффициент расхода (условный), кг топлива на (км * м/с²)
            const double fuelCoeff = 1000.0; // настраиваемый параметр
            // Расход топлива = коэффициент * изменение высоты (км) * g (м/с²)
            double fuel = fuelCoeff * deltaHeightKm * g;
            return fuel;
        }

        // Расчёт риска с учётом мусора
        public override float CalcRisk()
        {
            // Базовый риск (старая логика)
            float baseRisk;
            if (currHeight > 3000)
                baseRisk = 0.7f;
            else if (currHeight > 1000)
                baseRisk = 0.4f;
            else
                baseRisk = 0.2f;

            double debrisFactor = GetDebrisRiskFactor();
            float totalRisk = (float)(baseRisk * debrisFactor);
            return totalRisk > 1.0f ? 1.0f : totalRisk;
        }

        public override string GetInfo()
        {
            OrbitState ots = OrbitState();
            return $"{base.GetInfo()}\n" +
                   $"Mission type: Orbital\n" +
                   $"Orbital type: {ots.Type.ToString()}\n" +
                   $"Period: {ots.Period} min\n" +
                   $"Current orbital height: {currHeight} km\n" +
                   $"Target height: {targetHeight} km\n" +
                   $"Inclination: {inclination}°\n" +
                   $"Energy Source: {energySource.ToString()}";
        }
    }
}
