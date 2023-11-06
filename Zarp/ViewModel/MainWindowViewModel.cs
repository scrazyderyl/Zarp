using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zarp.Core;
using Zarp.ViewModel.MainWindow;

namespace Zarp.ViewModel
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

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            HomeVM = new HomeViewModel();
            FocusSessionVM = new FocusSessionViewModel();
            ScheduleVM = new ScheduleViewModel();
            RulesVM = new RulesViewModel();
            HistoryVM = new HistoryViewModel();
            SettingsVM = new SettingsViewModel();

            CurrentView = RulesVM;

            ChangeViewCommand = new RelayCommand(ChangeView);
        }

        public void ChangeView(object? parameter)
        {
            CurrentView = parameter;
        }
    }
}
