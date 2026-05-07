using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using SpaceMission.Models;

namespace SpaceMission.ViewModels
{
    public partial class MissionEditViewModel : ViewModelBase
    {
        private readonly Mission? _existingMission;
        private readonly IPlanetaryData _earthData;
        private readonly bool _isNewMode;

        [ObservableProperty] private string _name = string.Empty;

        [ObservableProperty] private int? _budget;          // nullable
        [ObservableProperty] private int? _duration;        // nullable

        // Orbital
        [ObservableProperty] private bool _isOrbital;
        [ObservableProperty] private double? _currHeight;    // nullable
        [ObservableProperty] private double? _targetHeight;  // nullable
        [ObservableProperty] private double? _inclination;   // nullable
        [ObservableProperty] private EnergySource _energySource;
        public List<EnergySource> EnergySources { get; } = new() { EnergySource.None, EnergySource.RTG, EnergySource.Solar };

        // Planetary
        [ObservableProperty] private bool _isPlanetary;
        [ObservableProperty] private string _planet = string.Empty;
        [ObservableProperty] private byte? _atmoDensity;     // nullable
        [ObservableProperty] private string _landingPointName = string.Empty;
        [ObservableProperty] private short? _landingPointX;  // nullable
        [ObservableProperty] private short? _landingPointY;  // nullable
        [ObservableProperty] private short? _landingPointR;  // nullable

        public IRelayCommand SaveCommand { get; }

        // Результат для новой миссии
        public Mission? ResultMission { get; private set; }

        public MissionEditViewModel(Mission? mission, IPlanetaryData earthData, bool isNew = false)
        {
            _earthData = earthData;
            _isNewMode = isNew;

            if (!isNew && mission != null)
            {
                _existingMission = mission;
                // Заполняем поля из существующей миссии
                Name = mission.Name;
                Budget = mission.Budget;
                Duration = mission.Duration;

                if (mission is OrbitalMission om)
                {
                    IsOrbital = true;
                    CurrHeight = om.CurrHeight;
                    TargetHeight = om.TargetHeight;
                    Inclination = om.Inclination;
                    EnergySource = om.EnergySource;
                }
                else if (mission is PlanetaryMission pm)
                {
                    IsPlanetary = true;
                    Planet = pm.Planet;
                    AtmoDensity = pm.AtmoDensity;
                    LandingPointName = pm.LandingPoint.Name;
                    LandingPointX = pm.LandingPoint.X;
                    LandingPointY = pm.LandingPoint.Y;
                    LandingPointR = pm.LandingPoint.R;
                }
            }
            else
            {
                // Новая миссия – все поля пустые (null или пустые строки)
                Name = string.Empty;
                Budget = null;
                Duration = null;
                if (IsOrbital)
                {
                    CurrHeight = null;
                    TargetHeight = null;
                    Inclination = null;
                    EnergySource = EnergySource.None; // значение по умолчанию, но пользователь может выбрать
                }
                if (IsPlanetary)
                {
                    Planet = string.Empty;
                    AtmoDensity = null;
                    LandingPointName = string.Empty;
                    LandingPointX = null;
                    LandingPointY = null;
                    LandingPointR = null;
                }
            }

            SaveCommand = new RelayCommand(Save);
        }

        private void Save()
        {
            if (_isNewMode)
            {
                // Создаём новую миссию на основе заполненных полей
                if (IsOrbital)
                    ResultMission = CreateOrbitalMission();
                else if (IsPlanetary)
                    ResultMission = CreatePlanetaryMission();
                else
                    return; // неизвестный тип
            }
            else
            {
                // Обновляем существующую миссию
                if (_existingMission != null)
                {
                    _existingMission.Name = Name;
                    if (Budget.HasValue) _existingMission.Budget = Budget.Value;
                    if (Duration.HasValue) _existingMission.Duration = Duration.Value;

                    if (_existingMission is OrbitalMission om)
                    {
                        if (CurrHeight.HasValue) om.CurrHeight = CurrHeight.Value;
                        if (TargetHeight.HasValue) om.TargetHeight = TargetHeight.Value;
                        if (Inclination.HasValue) om.Inclination = Inclination.Value;
                        om.EnergySource = EnergySource;
                    }
                    else if (_existingMission is PlanetaryMission pm)
                    {
                        if (!string.IsNullOrWhiteSpace(Planet)) pm.Planet = Planet;
                        if (AtmoDensity.HasValue) pm.AtmoDensity = AtmoDensity.Value;
                        pm.LandingPoint = new LandingPoint(
                            LandingPointName ?? string.Empty,
                            LandingPointX ?? 0,
                            LandingPointY ?? 0,
                            LandingPointR ?? 0);
                    }
                }
            }
        }

        private Mission CreateOrbitalMission()
        {
            // Проверяем, какие поля заполнены
            bool hasName = !string.IsNullOrWhiteSpace(Name);
            bool hasBudget = Budget.HasValue && Budget.Value > 0;
            bool hasDuration = Duration.HasValue && Duration.Value > 0;
            bool hasOrbitParams = CurrHeight.HasValue && TargetHeight.HasValue && Inclination.HasValue;

            if (hasName && hasBudget && hasDuration && hasOrbitParams)
            {
                // Полный конструктор
                return new OrbitalMission(
                    Name, Budget!.Value, Duration!.Value,
                    CurrHeight!.Value, TargetHeight!.Value, Inclination!.Value,
                    EnergySource, _earthData);
            }
            else if (hasName && hasBudget)
            {
                // Конструктор с именем и бюджетом (длительность 180, стандартные орбитальные параметры)
                return new OrbitalMission(Name, Budget!.Value);
            }
            else
            {
                // Конструктор по умолчанию
                return new OrbitalMission();
            }
        }

        private Mission CreatePlanetaryMission()
        {
            bool hasName = !string.IsNullOrWhiteSpace(Name);
            bool hasBudget = Budget.HasValue && Budget.Value > 0;
            bool hasDuration = Duration.HasValue && Duration.Value > 0;
            bool hasPlanet = !string.IsNullOrWhiteSpace(Planet);
            bool hasAtmo = AtmoDensity.HasValue;
            bool hasLanding = !string.IsNullOrWhiteSpace(LandingPointName) &&
                              LandingPointX.HasValue && LandingPointY.HasValue && LandingPointR.HasValue;

            if (hasName && hasBudget && hasDuration && hasPlanet && hasAtmo && hasLanding)
            {
                // Полный конструктор
                return new PlanetaryMission(
                    Name, Budget!.Value, Duration!.Value,
                    Planet, AtmoDensity!.Value,
                    new LandingPoint(LandingPointName!, LandingPointX!.Value, LandingPointY!.Value, LandingPointR!.Value));
            }
            else if (hasName && hasBudget && hasPlanet)
            {
                // Конструктор с именем, бюджетом и планетой (длительность 365, плотность атмосферы 100, точка посадки Default)
                return new PlanetaryMission(Name, Budget!.Value, Planet);
            }
            else
            {
                // Конструктор по умолчанию
                return new PlanetaryMission();
            }
        }
    }
}
