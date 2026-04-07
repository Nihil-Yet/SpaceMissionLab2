using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpaceMission.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using System;

namespace SpaceMission.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IMissionRepository _repository;
        private readonly IPlanetaryData _earthData;

        [ObservableProperty]
        private ObservableCollection<Mission> _missions = new();

        [ObservableProperty]
        private Mission _selectedMission;

        public MainWindowViewModel(IMissionRepository repository, IPlanetaryData earthData)
        {
            _repository = repository;
            _earthData = earthData;
            LoadMissionsCommand = new AsyncRelayCommand(LoadMissions);
            AddMissionCommand = new AsyncRelayCommand(AddMission);
            EditMissionCommand = new AsyncRelayCommand(EditMission, () => SelectedMission != null);
            DeleteMissionCommand = new AsyncRelayCommand(DeleteMission, () => SelectedMission != null);
            ShowDetailsCommand = new AsyncRelayCommand(ShowDetails, () => SelectedMission != null);

            LoadMissionsCommand.Execute(null);
        }

        public IAsyncRelayCommand LoadMissionsCommand { get; }
        public IAsyncRelayCommand AddMissionCommand { get; }
        public IAsyncRelayCommand EditMissionCommand { get; }
        public IAsyncRelayCommand DeleteMissionCommand { get; }
        public IAsyncRelayCommand ShowDetailsCommand { get; }

        private async Task LoadMissions()
        {
            var missions = await _repository.GetAllAsync();
            Missions.Clear();
            foreach (var m in missions)
                Missions.Add(m);
        }

        private async Task AddMission()
        {
            // Диалог выбора типа
            var dialog = new Views.MissionTypeDialog();
            var result = await dialog.ShowDialog<bool?>(GetWindow());
            if (result == true)
            {
                Mission newMission;
                if (dialog.SelectedType == "Orbital")
                    newMission = new OrbitalMission("New Orbital", 1000000, 180, 500, 500, 0, EnergySource.Solar, _earthData);
                else
                    newMission = new PlanetaryMission("New Planetary", 1000000, 365, "Mars", 100, new LandingPoint("Default", 0, 0, 0));

                var editVm = new MissionEditViewModel(newMission, _earthData);
                var editWindow = new Views.MissionEditWindow { DataContext = editVm };
                var editResult = await editWindow.ShowDialog<bool?>(GetWindow());
                if (editResult == true)
                {
                    await _repository.AddAsync(newMission);
                    await LoadMissions();
                }
            }
        }

        private async Task EditMission()
        {
            if (SelectedMission == null) return;
            var editVm = new MissionEditViewModel(SelectedMission, _earthData);
            var editWindow = new Views.MissionEditWindow { DataContext = editVm };
            var result = await editWindow.ShowDialog<bool?>(GetWindow());
            if (result == true)
            {
                await _repository.UpdateAsync(SelectedMission);
                await LoadMissions();
            }
        }

        private async Task DeleteMission()
        {
            if (SelectedMission == null) return;
            await _repository.DeleteAsync(SelectedMission.Id);
            await LoadMissions();
        }

        private async Task ShowDetails()
        {
            if (SelectedMission == null) return;
            var info = $"{SelectedMission.GetInfo()}\n\n" +
                       $"Risk: {SelectedMission.CalcRisk():P}\n" +
                       $"Fuel: {SelectedMission.CalcFuelConsumption():F2} units";
            var dialog = new Views.InfoDialog { DataContext = new { Info = info } };
            await dialog.ShowDialog(GetWindow());
        }

        private Window GetWindow()
        {
            return Avalonia.Application.Current?.ApplicationLifetime?
                .OfType<Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime>()
                .FirstOrDefault()?.MainWindow;
        }
    }
}
