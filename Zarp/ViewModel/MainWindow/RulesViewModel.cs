using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zarp.Core;
using Zarp.ViewModel.MainWindow.RulesEditor;

namespace Zarp.ViewModel.MainWindow
{
    internal class RulesViewModel : ObservableObject
    {
        public RulePresetsViewModel PresetsVM { get; set; }
        public GlobalRulesViewModel GlobalRulesVM { get; set; }
        public RewardsViewModel RewardsVM { get; set; }

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

        public RulesViewModel()
        {
            PresetsVM = new RulePresetsViewModel();
            GlobalRulesVM = new GlobalRulesViewModel();
            RewardsVM = new RewardsViewModel();

            CurrentView = PresetsVM;

            ChangeViewCommand = new RelayCommand(ChangeView);
        }

        public void ChangeView(object? parameter)
        {
            CurrentView = parameter;
        }
    }
}
