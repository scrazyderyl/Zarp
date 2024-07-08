using System.Collections.ObjectModel;
using Zarp.Core.App;
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
                Session.RemoveAlwaysAllowed(AllowedApplications[value]);
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
                Session.RemoveAlwaysBlocked(BlockedApplications[value]);
                BlockedApplications.RemoveAt(value);
            }
        }

        public GlobalRulesViewModel()
        {
            OpenApplicationSelectorCommand = new RelayCommand(OpenApplicationsSelector);

            AllowedApplications = new ObservableCollection<ApplicationInfo>(Session.AlwaysAllowedApplications._ApplicationRules);
            BlockedApplications = new ObservableCollection<ApplicationInfo>(Session.AlwaysBlockedApplications._ApplicationRules);
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
                Session.AddAlwaysAllowed(selector.Selected);
            }
            else
            {
                Session.AddAlwaysBlocked(selector.Selected);
            }

            AllowedApplications = new ObservableCollection<ApplicationInfo>(Session.AlwaysAllowedApplications._ApplicationRules);
            BlockedApplications = new ObservableCollection<ApplicationInfo>(Session.AlwaysBlockedApplications._ApplicationRules);

            OnPropertyChanged(nameof(AllowedApplications));
            OnPropertyChanged(nameof(BlockedApplications));
        }
    }
}
