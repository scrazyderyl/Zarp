using System.Collections.ObjectModel;
using Zarp.Core.Datatypes;
using Zarp.GUI.DataTypes;
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
                Core.App.Service.Blocker.RemoveAlwaysAllowed(AllowedApplications[value]);
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
                Core.App.Service.Blocker.RemoveAlwaysBlocked(BlockedApplications[value]);
                BlockedApplications.RemoveAt(value);
            }
        }

        public GlobalRulesViewModel()
        {
            OpenApplicationSelectorCommand = new RelayCommand(OpenApplicationsSelector);

            AllowedApplications = new ObservableCollection<ApplicationInfo>(Core.App.Service.Blocker.AlwaysAllowed.ApplicationRules);
            BlockedApplications = new ObservableCollection<ApplicationInfo>(Core.App.Service.Blocker.AlwaysBlocked.ApplicationRules);
            _SelectedAllowedApplicationIndex = -1;
            _SelectedBlockedApplicationIndex = -1;
        }

        void OpenApplicationsSelector(object? parameter)
        {
            ObservableCollection<ApplicationInfo> list = (ObservableCollection<ApplicationInfo>)parameter!;
            ApplicationSelectorView selector = new ApplicationSelectorView();
            selector.ShowDialog();

            if (!selector.Confirmed)
            {
                return;
            }

            if (list == AllowedApplications)
            {
                Core.App.Service.Blocker.AddAlwaysAllowed(selector.Selected);
            }
            else
            {
                Core.App.Service.Blocker.AddAlwaysBlocked(selector.Selected);
            }

            AllowedApplications = new ObservableCollection<ApplicationInfo>(Core.App.Service.Blocker.AlwaysAllowed.ApplicationRules);
            BlockedApplications = new ObservableCollection<ApplicationInfo>(Core.App.Service.Blocker.AlwaysBlocked.ApplicationRules);

            OnPropertyChanged(nameof(AllowedApplications));
            OnPropertyChanged(nameof(BlockedApplications));
        }
    }
}
