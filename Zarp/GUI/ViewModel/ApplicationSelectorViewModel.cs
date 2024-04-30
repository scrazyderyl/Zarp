using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Zarp.Core.Datatypes;
using Zarp.GUI.DataTypes;
using Zarp.GUI.Model;
using Zarp.GUI.Util;

namespace Zarp.GUI.ViewModel
{
    internal class ApplicationSelectorViewModel : ObservableObject
    {
        public HashSet<string> SystemApplications = new HashSet<string>(new string[]
        {
            "control",
            "appverif",
            "charmap",
            "powershell",
            "dfrgui",
            "cleanmgr",
            "Control",
            "cmd",
            "iexplore",
            "iscsicpl",
            "magnify",
            "mip",
            "MdSched",
            "msedge",
            "narrator",
            "notepad",
            "odbcad32",
            "SETLANG",
            "osk",
            "mspaint",
            "PCHealthCheck",
            "hh",
            "quickassist",
            "RecoveryDrive",
            "regedit",
            "mstsc",
            "perfmon",
            "SnippingTool",
            "sapisvr",
            "psr",
            "msconfig",
            "msinfo32",
            "taskmgr",
            "msoev",
            "appcertui",
            "WFS",
            "wmplayer",
            "PowerShell_ISE",
            "explorer",
            "wordpad"
        });

        public RelayCommand SelectExecutableCommand { get; set; }

        public ObservableCollection<ItemWithIcon<ApplicationInfo>> InstalledApplications { get; set; }
        private string? _InstalledApplicationsQuery;
        public string? InstalledApplicationsQuery
        {
            get { return _InstalledApplicationsQuery; }
            set
            {
                _InstalledApplicationsQuery = value;

                if (!InstalledApplicationsQueryIsEmpty)
                {
                    UpdateInstalledApplications();
                }
            }
        }
        public bool InstalledApplicationsQueryIsEmpty { get; set; }
        private bool _HideSystemApplications;
        public bool HideSystemApplications
        {
            get { return _HideSystemApplications; }
            set
            {
                _HideSystemApplications = value;
                UpdateInstalledApplications();
            }
        }
        public ObservableCollection<ItemWithIcon<ApplicationInfo>> OpenApplications { get; set; }
        public ObservableCollection<ItemWithIcon<ApplicationInfo>> OtherApplications { get; set; }
        private int _OtherApplicationsSelectedIndex;
        public int OtherApplicationsSelectedIndex
        {
            get => _OtherApplicationsSelectedIndex;
            set
            {
                _OtherApplicationsUnique.Remove(OtherApplications[value].Data);
                OtherApplications.RemoveAt(value);
                _OtherApplicationsSelectedIndex = -1;
            }
        }

        private HashSet<ApplicationInfo> _OtherApplicationsUnique;

        public ApplicationSelectorViewModel()
        {
            _HideSystemApplications = false;
            InstalledApplications = new ObservableCollection<ItemWithIcon<ApplicationInfo>>(GetFilteredApplications());

            OpenApplications = new ObservableCollection<ItemWithIcon<ApplicationInfo>>(ApplicationSelectorModel.OpenApplications);

            _OtherApplicationsUnique = new HashSet<ApplicationInfo>();
            _OtherApplicationsSelectedIndex = -1;
            OtherApplications = new ObservableCollection<ItemWithIcon<ApplicationInfo>>();

            SelectExecutableCommand = new RelayCommand(SelectExecutable);
        }

        public void Search(string query)
        {
            _InstalledApplicationsQuery = query;
            UpdateInstalledApplications();
        }

        private IEnumerable<ItemWithIcon<ApplicationInfo>> GetFilteredApplications()
        {
            IEnumerable<ItemWithIcon<ApplicationInfo>> ApplicationList = ApplicationSelectorModel.InstalledApplications;

            if (_HideSystemApplications)
            {
                ApplicationList = ApplicationList.Where(application => !SystemApplications.Contains(application.Data.FileName));
            }

            if (_InstalledApplicationsQuery != null)
            {
                string query = _InstalledApplicationsQuery.Trim().ToLowerInvariant();
                ApplicationList = ApplicationList.Where(application => application.Data.Name.ToLowerInvariant().Contains(query));
            }

            return ApplicationList;
        }

        private void UpdateInstalledApplications()
        {
            InstalledApplications = new ObservableCollection<ItemWithIcon<ApplicationInfo>>(GetFilteredApplications());
            OnPropertyChanged(nameof(InstalledApplications));
        }

        private void SelectExecutable(object? obj)
        {
            string[] paths = FileDialogs.OpenExeMulti();

            foreach (string path in paths)
            {
                ApplicationInfo info = new ApplicationInfo(path);

                if (_OtherApplicationsUnique.Add(info))
                {
                    OtherApplications.Add(new ItemWithIcon<ApplicationInfo>(info, info.GetIconAsBitmapSource()));
                }

            }
        }
    }
}
