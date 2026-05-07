using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpaceMission.Models;
using System;

namespace SpaceMission.ViewModels
{
    public partial class InfoDialogViewModel : ViewModelBase
    {
        private readonly Mission _mission;

        [ObservableProperty] private string _title = "Информация о миссии";
        [ObservableProperty] private string _content = string.Empty;

        public IRelayCommand ShowGetInfoCommand { get; }
        public IRelayCommand ShowCalcRiskCommand { get; }
        public IRelayCommand ShowCalcFuelCommand { get; }
        public IRelayCommand ShowOrbitStateCommand { get; }
        public IRelayCommand ShowHasAtmosphereCommand { get; }
        public IRelayCommand ShowScientificGoalCommand { get; }

        public bool IsOrbital => _mission is OrbitalMission;
        public bool IsPlanetary => _mission is PlanetaryMission;

        public InfoDialogViewModel(Mission mission)
        {
            _mission = mission;

            ShowGetInfoCommand = new RelayCommand(ShowGetInfo);
            ShowCalcRiskCommand = new RelayCommand(ShowCalcRisk);
            ShowCalcFuelCommand = new RelayCommand(ShowCalcFuel);

            if (IsOrbital)
                ShowOrbitStateCommand = new RelayCommand(ShowOrbitState);
            if (IsPlanetary)
            {
                ShowHasAtmosphereCommand = new RelayCommand(ShowHasAtmosphere);
                ShowScientificGoalCommand = new RelayCommand(ShowScientificGoal);
            }

            // Показываем GetInfo по умолчанию
            ShowGetInfo();
        }

        private void ShowGetInfo()
        {
            Title = "📄 Общая информация (GetInfo)";
            Content = _mission.GetInfo();
        }

        private void ShowCalcRisk()
        {
            Title = "⚠️ Риск миссии (CalcRisk)";
            Content = $"Риск: {_mission.CalcRisk():P}";
        }

        private void ShowCalcFuel()
        {
            Title = "⛽ Расход топлива (CalcFuelConsumption)";
            Content = $"Расход топлива: {_mission.CalcFuelConsumption():F2} ед.";
        }

        private void ShowOrbitState()
        {
            if (_mission is OrbitalMission om)
            {
                var state = om.OrbitState();
                Title = "🛰️ Состояние орбиты (OrbitState)";
                Content = $"Высота: {state.CurrHeight} км\n" +
                          $"Тип орбиты: {state.Type}\n" +
                          $"Наклонение: {state.Inclination}°\n" +
                          $"Период: {state.Period:F1} мин";
            }
        }

        private void ShowHasAtmosphere()
        {
            if (_mission is PlanetaryMission pm)
            {
                Title = "🌍 Наличие атмосферы (HasAtmosphere)";
                Content = pm.HasAtmosphere() ? "Атмосфера есть" : "Атмосферы нет";
            }
        }

        private void ShowScientificGoal()
        {
            if (_mission is PlanetaryMission pm)
            {
                Title = "🔬 Научная цель (ScientificGoal)";
                Content = pm.ScientificGoal();
            }
        }
    }
}
