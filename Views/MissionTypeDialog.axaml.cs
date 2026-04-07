using Avalonia.Controls;
using Avalonia.Interactivity;

namespace SpaceMission.Views
{
    public partial class MissionTypeDialog : Window
    {
        public string SelectedType { get; private set; } = "Orbital";

        public MissionTypeDialog()
        {
            InitializeComponent();
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            if (TypeCombo.SelectedItem is ComboBoxItem item)
                SelectedType = item.Content?.ToString() ?? "Orbital";
            Close(true);
        }
    }
}
