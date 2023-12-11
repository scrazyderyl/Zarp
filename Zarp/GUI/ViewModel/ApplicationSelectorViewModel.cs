using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.IO;
using static Zarp.GUI.Model.ApplicationSelectorModel;
using Zarp.GUI.View;
using System.Collections.Generic;
using System.Linq;
using Zarp.Core.Datatypes;
using Zarp.GUI.Util;
using System.Windows.Media.Imaging;

namespace Zarp.GUI.ViewModel
{

    internal class ApplicationSelectorViewModel : ObservableObject
    {
        private static string WindowsPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

        public RelayCommand SelectExecutableCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand DoneCommand { get; set; }

        private List<ItemWithIcon<ApplicationInfo>> _AllInstalledApplications;
        public ObservableCollection<ItemWithIcon<ApplicationInfo>> InstalledApplications { get; set; }
        private string? _InstalledApplicationsQuery;
        public string? InstalledApplicationsQuery
        {
            get { return _InstalledApplicationsQuery; }
            set
            {
                _InstalledApplicationsQuery = value;
                UpdateInstalledApplications();
            }
        }
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
        public ObservableCollection<ApplicationInfo> OpenApplications { get; set; }
        public ObservableCollection<ApplicationInfo> OtherApplications { get; set; }

        public ApplicationSelectorViewModel()
        {
            _HideSystemApplications = true;

            _AllInstalledApplications = new List<ItemWithIcon<ApplicationInfo>>(GetInstalledApplications().Select(application =>
            {
                Core.Service.Zarp.IconCache.Get(application.ExecutablePath, out BitmapSource? icon);
                return new ItemWithIcon<ApplicationInfo>(application, icon);
            }));

            UpdateInstalledApplications();

            OpenApplications = new ObservableCollection<ApplicationInfo>(GetOpenApplications());

            OtherApplications = new ObservableCollection<ApplicationInfo>();

            SelectExecutableCommand = new RelayCommand(SelectExecutable);
            CancelCommand = new RelayCommand(Cancel);
            DoneCommand = new RelayCommand(Done);
        }

        private void UpdateInstalledApplications()
        {
            IEnumerable<ItemWithIcon<ApplicationInfo>> ApplicationList = _AllInstalledApplications;

            if (_HideSystemApplications)
            {
                ApplicationList = ApplicationList.Where(application => !application.Data.ExecutablePath.Contains(WindowsPath));
            }

            if (_InstalledApplicationsQuery != null)
            {
                string query = _InstalledApplicationsQuery.Trim().ToLowerInvariant();
                ApplicationList = ApplicationList.Where(application => application.Data.Name.ToLowerInvariant().Contains(query));
            }

            InstalledApplications = new ObservableCollection<ItemWithIcon<ApplicationInfo>>(ApplicationList);
            OnPropertyChanged("InstalledApplications");
        }

        private void SelectExecutable(object? obj)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = ".exe";
            fileDialog.Filter = "Executable files (*.exe)|*.exe";
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + @"\Programs";
            fileDialog.Multiselect = true;
            fileDialog.ShowDialog();

            foreach (string fileName in fileDialog.FileNames)
            {
                FileInfo fileInfo = new FileInfo(fileName);
                string name = fileInfo.Name.Substring(0, fileInfo.Name.Length - 4);
                ApplicationInfo info = new ApplicationInfo(fileInfo.FullName, name);

                if (!OtherApplications.Contains(info))
                {
                    OtherApplications.Add(info);
                }
            }
        }

        private void Cancel(object? obj)
        {
            ((ApplicationSelectorView)obj!).Close();
        }

        private void Done(object? obj)
        {
            ((ApplicationSelectorView)obj!).Close();
        }
    }
}
