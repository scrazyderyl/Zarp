using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        public RelayCommand RestoreWindowCommand { get; set; }
        public RelayCommand ChangeViewCommand { get; set; }

        public object? CurrentView {  get; set; }

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
            RestoreWindowCommand = new RelayCommand(RestoreWindow);
        }

        public void RestoreWindow(object? parameter)
        {
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        public void ChangeView(object? parameter)
        {
            CurrentView = parameter;
            OnPropertyChanged("CurrentView");
        }
    }
}
