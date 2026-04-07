using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace SpaceMission.ViewModels
{
    public partial class MissionEditViewModel : ViewModelBase
    {
        private readonly Mission _mission;
        private readonly IPlanetaryData _earthData;

        [ObservableProperty] private string _name;
        [ObservableProperty] private int _budget;
        [ObservableProperty] private int _duration;

        // Orbital
        [ObservableProperty] private bool _isOrbital;
        [ObservableProperty] private double _currHeight;
        [ObservableProperty] private double _targetHeight;
        [ObservableProperty] private double _inclination;
        [ObservableProperty] private EnergySource _energySource;

        // Planetary
        [ObservableProperty] private bool _isPlanetary;
        [ObservableProperty] private string _planet;
        [ObservableProperty] private byte _atmoDensity;
        [ObservableProperty] private string _landingPointName;
        [ObservableProperty] private short _landingPointX;
        [ObservableProperty] private short _landingPointY;
        [ObservableProperty] private short _landingPointR;

        public MissionEditViewModel(Mission mission, IPlanetaryData earthData)
        {
            _mission = mission;
            _earthData = earthData;

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

        [RelayCommand]
        private void Save()
        {
            _mission.Name = Name;
            _mission.Budget = Budget;
            _mission.Duration = Duration;

            if (_mission is OrbitalMission om)
            {
                om.CurrHeight = CurrHeight;
                om.TargetHeight = TargetHeight;
                om.Inclination = Inclination;
                om.EnergySource = EnergySource;
                // planetaryData не сохраняется в БД, но нужно для расчётов
                // оно уже установлено в репозитории
            }
            else if (_mission is PlanetaryMission pm)
            {
                pm.Planet = Planet;
                pm.AtmoDensity = AtmoDensity;
                pm.LandingPoint = new LandingPoint(LandingPointName, LandingPointX, LandingPointY, LandingPointR);
            }
        }
    }
}
