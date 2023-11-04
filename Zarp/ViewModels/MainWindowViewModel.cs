using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Zarp.Core;
using Zarp.Views;

namespace Zarp.ViewModels
{
    class MainWindowViewModel : ObservableObject
    {
		public RelayCommand HomeViewCommand;
		public RelayCommand FocusSessionViewCommand;
		public RelayCommand ScheduleViewCommand;
		public RelayCommand RulesViewCommand;
		public RelayCommand SettingsViewCommand;

		public HomeViewModel HomeVM { get; set; }
		public FocusSessionViewModel FocusSessionVM { get; set; }
		public ScheduleViewModel ScheduleVM { get; set; }
		public RulesViewModel RulesVM { get; set; }
		public SettingsViewModel SettingsVM { get; set; }

		private object _currentView;

		public object CurrentView
		{
			get { return _currentView; }
			set {
				_currentView = value;
				OnPropertyChanged();
			}
		}

		public MainWindowViewModel() {
			HomeVM = new HomeViewModel();
			FocusSessionVM = new FocusSessionViewModel();
			ScheduleVM = new ScheduleViewModel();
			RulesVM = new RulesViewModel();
			SettingsVM = new SettingsViewModel();

			_currentView = HomeVM;

			HomeViewCommand = new RelayCommand(obj => { CurrentView = HomeVM; });
            FocusSessionViewCommand = new RelayCommand(obj => { CurrentView = FocusSessionVM; });
			ScheduleViewCommand = new RelayCommand(obj => { CurrentView = ScheduleVM; });
            RulesViewCommand = new RelayCommand(_Execute => { CurrentView = RulesVM; });
            SettingsViewCommand = new RelayCommand(_Execute => { CurrentView = SettingsVM; });
        }
    }
}
