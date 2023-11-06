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

        public ObservableCollection<string> AllowedApplications { get; set; }
        public ObservableCollection<string> BlockedApplications { get; set; }

        public GlobalRulesViewModel()
        {
            AllowedApplications = new ObservableCollection<string>();
            BlockedApplications = new ObservableCollection<string>();

            OpenApplicationSelectorCommand = new RelayCommand(OpenApplicationsSelector);
        }

        void OpenApplicationsSelector(object? parameter)
        {
            new ApplicationSelectorView().ShowDialog();
        }
    }
}
