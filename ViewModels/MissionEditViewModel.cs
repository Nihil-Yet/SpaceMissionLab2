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

        // User mode
        [ObservableProperty] private bool _isDevMode = false;
        public bool IsNormalMode => !IsDevMode;

        // Для отображения в обычном режиме
        public string ReadOnlyName => _existingMission?.Name ?? string.Empty;
        public int ReadOnlyBudget => _existingMission?.Budget ?? 0;
        public int ReadOnlyDuration => _existingMission?.Duration ?? 0;
        public double ReadOnlyCurrHeight => (_existingMission as OrbitalMission)?.CurrHeight ?? 0;
        public double ReadOnlyTargetHeight => (_existingMission as OrbitalMission)?.TargetHeight ?? 0;

        // Параметры для команд
        [ObservableProperty] private int _extendDays;
        [ObservableProperty] private double _orbitDelta;
        [ObservableProperty] private double _newTargetHeight;
        [ObservableProperty] private string _newLandingName = string.Empty;
        [ObservableProperty] private short _newLandingX;
        [ObservableProperty] private short _newLandingY;
        [ObservableProperty] private short _newLandingR;

        // Команды для обычного режима
        public IRelayCommand ExtendMissionCommand { get; }
        public IRelayCommand ChangeOrbitCommand { get; }      // только для Orbital
        public IRelayCommand SetTargetHeightCommand { get; }  // только для Orbital
        public IRelayCommand SetLandingPointCommand { get; }   // только для Planetary

        // DevMode поля
        [ObservableProperty] private string _name = string.Empty;
        [ObservableProperty] private int? _budget;
        [ObservableProperty] private int? _duration;

        // Orbital
        [ObservableProperty] private bool _isOrbital;
        [ObservableProperty] private double? _currHeight;
        [ObservableProperty] private double? _targetHeight;
        [ObservableProperty] private double? _inclination;
        [ObservableProperty] private EnergySource _energySource;
        public List<EnergySource> EnergySources { get; } = new() { EnergySource.None, EnergySource.RTG, EnergySource.Solar };

        // Planetary
        [ObservableProperty] private bool _isPlanetary;
        [ObservableProperty] private string _planet = string.Empty;
        [ObservableProperty] private byte? _atmoDensity;
        [ObservableProperty] private string _landingPointName = string.Empty;
        [ObservableProperty] private short? _landingPointX;
        [ObservableProperty] private short? _landingPointY;
        [ObservableProperty] private short? _landingPointR;

        // Команда сохранения
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
                // Новая миссия – все поля пустые
                Name = string.Empty;
                Budget = null;
                Duration = null;
                if (IsOrbital)
                {
                    CurrHeight = null;
                    TargetHeight = null;
                    Inclination = null;
                    EnergySource = EnergySource.None;
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
            ExtendMissionCommand = new RelayCommand(ExtendMission);
            ChangeOrbitCommand = new RelayCommand(ChangeOrbit, () => IsOrbital);
            SetTargetHeightCommand = new RelayCommand(SetTargetHeight, () => IsOrbital);
            SetLandingPointCommand = new RelayCommand(SetLandingPoint, () => IsPlanetary);
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
                    return;
            }
            else
            {
                // Обновляем существующую миссию (DevMode)
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

        partial void OnIsDevModeChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNormalMode));
        }

        private void ExtendMission()
        {
            if (_existingMission != null && ExtendDays > 0)
                _existingMission.ExtendMission(ExtendDays);
            OnPropertyChanged(nameof(ReadOnlyDuration));
        }

        private void ChangeOrbit()
        {
            if (_existingMission is OrbitalMission om && OrbitDelta != 0)
                om.ChangeOrbit(OrbitDelta);
            OnPropertyChanged(nameof(ReadOnlyCurrHeight));
        }

        private void SetTargetHeight()
        {
            if (_existingMission is OrbitalMission om && NewTargetHeight > 0)
                om.SetTargetHeight(NewTargetHeight);
            OnPropertyChanged(nameof(ReadOnlyTargetHeight));
        }

        private void SetLandingPoint()
        {
            if (_existingMission is PlanetaryMission pm)
            {
                var newPoint = new LandingPoint(NewLandingName, NewLandingX, NewLandingY, NewLandingR);
                pm.SetLandingPoint(newPoint);
            }
        }

        private Mission CreateOrbitalMission()
        {
            bool hasName = !string.IsNullOrWhiteSpace(Name);
            bool hasBudget = Budget.HasValue && Budget.Value > 0;
            bool hasDuration = Duration.HasValue && Duration.Value > 0;
            bool hasOrbitParams = CurrHeight.HasValue && TargetHeight.HasValue && Inclination.HasValue;

            if (hasName && hasBudget && hasDuration && hasOrbitParams)
            {
                return new OrbitalMission(
                    Name, Budget!.Value, Duration!.Value,
                    CurrHeight!.Value, TargetHeight!.Value, Inclination!.Value,
                    EnergySource, _earthData);
            }
            else if (hasName && hasBudget)
            {
                return new OrbitalMission(Name, Budget!.Value);
            }
            else
            {
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
                return new PlanetaryMission(
                    Name, Budget!.Value, Duration!.Value,
                    Planet, AtmoDensity!.Value,
                    new LandingPoint(LandingPointName!, LandingPointX!.Value, LandingPointY!.Value, LandingPointR!.Value));
            }
            else if (hasName && hasBudget && hasPlanet)
            {
                return new PlanetaryMission(Name, Budget!.Value, Planet);
            }
            else
            {
                return new PlanetaryMission();
            }
        }
    }
}
