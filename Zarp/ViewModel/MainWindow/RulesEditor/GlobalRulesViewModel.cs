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
    internal class GlobalRulesViewModel : ObservableObject
    {
        public RelayCommand OpenApplicationSelectorCommand { get; set; }

        public ObservableCollection<ApplicationInfo> AllowedApplications { get; set; }
        public ObservableCollection<ApplicationInfo> BlockedApplications { get; set; }

        public GlobalRulesViewModel()
        {
            OpenApplicationSelectorCommand = new RelayCommand(OpenApplicationsSelector);

            AllowedApplications = new ObservableCollection<ApplicationInfo>(Zarp.Core.Zarp.Blocker.GetAlwaysAllowedApplications());
            BlockedApplications = new ObservableCollection<ApplicationInfo>(Zarp.Core.Zarp.Blocker.GetAlwaysBlockedApplications());
        }

        void OpenApplicationsSelector(object? parameter)
        {
            ObservableCollection<ApplicationInfo> list = (ObservableCollection<ApplicationInfo>)parameter;
            Zarp.Core.Zarp.DialogReturnValue = null;
            new ApplicationSelectorView().ShowDialog();

            List<ApplicationInfo>? newRules = (List<ApplicationInfo>?)Zarp.Core.Zarp.DialogReturnValue;

            if (newRules == null)
            {
                return;
            }

            if (list == AllowedApplications)
            {
                Zarp.Core.Zarp.Blocker.AddAlwaysAllowedApplications(newRules);
            }
            else
            {
                Zarp.Core.Zarp.Blocker.AddAlwaysBlockedApplications(newRules);
            }

            AllowedApplications = new ObservableCollection<ApplicationInfo>(Zarp.Core.Zarp.Blocker.GetAlwaysAllowedApplications());
            BlockedApplications = new ObservableCollection<ApplicationInfo>(Zarp.Core.Zarp.Blocker.GetAlwaysBlockedApplications());

            OnPropertyChanged("AllowedApplications");
            OnPropertyChanged("BlockedApplications");
        }
    }
}
