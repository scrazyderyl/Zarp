using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Zarp.Core.Datatypes;
using Zarp.GUI.Util;
using Zarp.GUI.View;

namespace Zarp.GUI.ViewModel.MainWindow.RulesEditor
{
    internal class GlobalRulesViewModel : ObservableObject
    {
        public RelayCommand OpenApplicationSelectorCommand { get; set; }

        public ObservableCollection<ApplicationInfo> AllowedApplications { get; set; }
        private int _SelectedAllowedApplicationIndex;
        public int SelectedAllowedApplicationIndex
        {
            get { return _SelectedAllowedApplicationIndex; }
            set
            {
                Core.Service.Zarp.Blocker.RemoveAlwaysAllowedApplication(AllowedApplications[value]);
                AllowedApplications.RemoveAt(value);
            }
        }
        public ObservableCollection<ApplicationInfo> BlockedApplications { get; set; }
        private int _SelectedBlockedApplicationIndex;
        public int SelectedBlockedApplicationIndex
        {
            get { return _SelectedBlockedApplicationIndex; }
            set
            {
                Core.Service.Zarp.Blocker.RemoveAlwaysBlockedApplication(BlockedApplications[value]);
                BlockedApplications.RemoveAt(value);
            }
        }

        public GlobalRulesViewModel()
        {
            OpenApplicationSelectorCommand = new RelayCommand(OpenApplicationsSelector);

            AllowedApplications = new ObservableCollection<ApplicationInfo>(Core.Service.Zarp.Blocker.AlwaysAllowed.ApplicationRules.GetRules());
            BlockedApplications = new ObservableCollection<ApplicationInfo>(Core.Service.Zarp.Blocker.AlwaysBlocked.ApplicationRules.GetRules());
            _SelectedAllowedApplicationIndex = -1;
            _SelectedBlockedApplicationIndex = -1;
        }

        void OpenApplicationsSelector(object? parameter)
        {
            ObservableCollection<ApplicationInfo> list = (ObservableCollection<ApplicationInfo>)parameter!;
            Core.Service.Zarp.DialogReturnValue = null;
            new ApplicationSelectorView().ShowDialog();

            List<ApplicationInfo>? newRules = (List<ApplicationInfo>?)Core.Service.Zarp.DialogReturnValue;

            if (newRules == null)
            {
                return;
            }

            if (list == AllowedApplications)
            {
                Core.Service.Zarp.Blocker.AddAlwaysAllowedApplications(newRules);
            }
            else
            {
                Core.Service.Zarp.Blocker.AddAlwaysBlockedApplications(newRules);
            }

            AllowedApplications = new ObservableCollection<ApplicationInfo>(Core.Service.Zarp.Blocker.AlwaysAllowed.ApplicationRules.GetRules());
            BlockedApplications = new ObservableCollection<ApplicationInfo>(Core.Service.Zarp.Blocker.AlwaysBlocked.ApplicationRules.GetRules());

            OnPropertyChanged("AllowedApplications");
            OnPropertyChanged("BlockedApplications");
        }
    }
}
