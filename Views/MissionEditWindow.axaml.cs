using Avalonia.Controls;
using Avalonia.Interactivity;
using SpaceMission.ViewModels;

namespace SpaceMission.Views
{
    public partial class MissionEditWindow : Window
    {
        public MissionEditWindow()
        {
            InitializeComponent();
        }

        private async void OkClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is MissionEditViewModel vm)
            {
                if (vm.IsDevMode)
                {
                    vm.SaveCommand.Execute(null);
                }
            }
            Close(true);
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close(false);
        }
    }
}
