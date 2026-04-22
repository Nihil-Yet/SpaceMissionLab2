using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace SpaceMission.Views
{
    public partial class MissionTypeDialog : Window
    {
        private ComboBox? _typeCombo;
        public string SelectedType { get; private set; } = "Orbital";

        public MissionTypeDialog()
        {
            AvaloniaXamlLoader.Load(this);
            _typeCombo = this.FindControl<ComboBox>("TypeCombo");
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            if (_typeCombo?.SelectedItem is ComboBoxItem item)
                SelectedType = item.Content?.ToString() ?? "Orbital";
            Close(true);
        }
    }
}
