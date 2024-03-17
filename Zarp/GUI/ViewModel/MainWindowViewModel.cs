using Zarp.GUI.DataTypes;
using Zarp.GUI.ViewModel.MainWindow;

namespace Zarp.GUI.ViewModel
{
    internal class MainWindowViewModel : ObservableObject
    {
        public HomeViewModel HomeVM { get; set; }
        public FocusSessionViewModel FocusSessionVM { get; set; }
        public ScheduleViewModel ScheduleVM { get; set; }
        public RulesViewModel RulesVM { get; set; }
        public HistoryViewModel HistoryVM { get; set; }
        public SettingsViewModel SettingsVM { get; set; }

        public RelayCommand ChangeViewCommand { get; set; }

        public object? CurrentView { get; set; }

        public MainWindowViewModel()
        {
            HomeVM = new HomeViewModel();
            FocusSessionVM = new FocusSessionViewModel();
            ScheduleVM = new ScheduleViewModel();
            RulesVM = new RulesViewModel();
            HistoryVM = new HistoryViewModel();
            SettingsVM = new SettingsViewModel();

            CurrentView = HomeVM;

            ChangeViewCommand = new RelayCommand(ChangeView);
        }

        public void ChangeView(object? parameter)
        {
            CurrentView = parameter!;
            OnPropertyChanged(nameof(CurrentView));
        }
    }
}
