using Avalonia.ReactiveUI;
using SpaceMission.ViewModels;

namespace SpaceMission.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
