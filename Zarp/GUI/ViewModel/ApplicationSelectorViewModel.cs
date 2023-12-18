using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Zarp.Core.Datatypes;
using Zarp.GUI.Model;
using Zarp.GUI.Util;

namespace Zarp.GUI.ViewModel
{

    internal class ApplicationSelectorViewModel : ObservableObject
    {
        private static string[] _WindowsPaths = new string[]
        {
            Environment.GetFolderPath(Environment.SpecialFolder.Windows),
            Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles)
        };

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
                _OtherApplicationsUnique.Remove(OtherApplications[value].Data.ExecutablePath);
                OtherApplications.RemoveAt(value);
                _OtherApplicationsSelectedIndex = -1;
            }
        }
        private HashSet<string> _OtherApplicationsUnique;

        public ApplicationSelectorViewModel()
        {
            _HideSystemApplications = true;
            InstalledApplications = new ObservableCollection<ItemWithIcon<ApplicationInfo>>(GetFilteredApplications());

            OpenApplications = new ObservableCollection<ItemWithIcon<ApplicationInfo>>(ApplicationSelectorModel.OpenApplications);

            _OtherApplicationsUnique = new HashSet<string>();
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
                ApplicationList = ApplicationList.Where(application =>
                {
                    string lowerPath = application.Data.ExecutablePath;

                    foreach (string excludedPath in _WindowsPaths)
                    {
                        if (lowerPath.Contains(excludedPath))
                        {
                            return false;
                        }
                    }

                    return true;
                });
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
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                DefaultExt = ".exe",
                Filter = "Executable files (*.exe)|*.exe",
                Multiselect = true
            };

            fileDialog.ShowDialog();

            foreach (string fileName in fileDialog.FileNames)
            {
                if (!_OtherApplicationsUnique.Add(fileName))
                {
                    continue;
                }

                string name = Path.GetFileName(fileName);
                name = name.Substring(0, name.Length - 4);
                ApplicationInfo application = new ApplicationInfo(fileName, name);
                BitmapSource? icon = ApplicationSelectorModel.GetExecutableIcon(fileName);
                OtherApplications.Add(new ItemWithIcon<ApplicationInfo>(application, icon));
            }
        }
    }
}
