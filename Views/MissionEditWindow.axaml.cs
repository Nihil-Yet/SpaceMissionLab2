using Avalonia.Controls;
using Avalonia.Interactivity;

namespace SpaceMission.Views
{
    public partial class MissionEditWindow : Window
    {
        public MissionEditWindow()
        {
            InitializeComponent();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close(false);
        }
    }
}
