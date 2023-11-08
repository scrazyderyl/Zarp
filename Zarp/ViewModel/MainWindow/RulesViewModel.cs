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
        public RulePresetsViewModel PresetsVM { get; }
        public GlobalRulesViewModel GlobalRulesVM { get; }
        public RewardsViewModel RewardsVM { get; }

        public RelayCommand ChangeViewCommand { get; set; }

        public object CurrentView { get; set; }

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
            OnPropertyChanged("CurrentView");
        }
    }
}
