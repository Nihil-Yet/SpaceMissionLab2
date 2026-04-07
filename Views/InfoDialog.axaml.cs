using Avalonia.Controls;
using Avalonia.Interactivity;

namespace SpaceMission.Views
{
    public partial class InfoDialog : Window
    {
        public InfoDialog()
        {
            InitializeComponent();
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
