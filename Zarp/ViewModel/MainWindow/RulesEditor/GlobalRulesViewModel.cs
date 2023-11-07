using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Zarp.Core;
using Zarp.View;

namespace Zarp.ViewModel.MainWindow.RulesEditor
{
    internal class GlobalRulesViewModel
    {
        public RelayCommand OpenApplicationSelectorCommand { get; set; }

        public static ObservableCollection<ApplicationInfo> AllowedApplications { get; } = new ObservableCollection<ApplicationInfo>();
        public static ObservableCollection<ApplicationInfo> BlockedApplications { get; } = new ObservableCollection<ApplicationInfo>();

        public GlobalRulesViewModel()
        {
            OpenApplicationSelectorCommand = new RelayCommand(OpenApplicationsSelector);
        }

        void OpenApplicationsSelector(object? parameter)
        {
            Zarp.Core.Zarp.CurrentRuleset = (ObservableCollection<ApplicationInfo>)parameter;
            new ApplicationSelectorView().ShowDialog();
        }
    }
}
