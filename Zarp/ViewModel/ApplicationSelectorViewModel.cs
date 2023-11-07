using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Zarp.Core.Util;
using Zarp.Core;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices.ObjectiveC;
using System.Windows.Controls;
using System.IO;
using Zarp.View;

namespace Zarp.ViewModel
{

    internal class ApplicationSelectorViewModel
    {
        public RelayCommand SelectExecutableCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand DoneCommand { get; set; }

        public ObservableCollection<ApplicationInfo> OpenApplications { get; set; }
        public ObservableCollection<ApplicationInfo> InstalledApplications { get; set; }
        public ObservableCollection<ApplicationInfo> UserSpecifiedApplications { get; set; }

        public ApplicationSelectorViewModel()
        {
            OpenApplications = new ObservableCollection<ApplicationInfo>();
            InstalledApplications = new ObservableCollection<ApplicationInfo>();
            UserSpecifiedApplications = new ObservableCollection<ApplicationInfo>();

            foreach (ApplicationInfo app in GetOpenWindows())
            {
                OpenApplications.Add(app);
            }

            foreach (ApplicationInfo app in GetStartMenuApplications())
            {
                InstalledApplications.Add(app);
            }

            SelectExecutableCommand = new RelayCommand(SelectExecutable);
            CancelCommand = new RelayCommand(Cancel);
            DoneCommand = new RelayCommand(Done);
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

                if (!UserSpecifiedApplications.Contains(info))
                {
                    UserSpecifiedApplications.Add(info);
                }
            }
        }

        private void Cancel(object? obj)
        {
            ((ApplicationSelectorView)obj).Close();
        }

        private void Done(object? obj)
        {
            ((ApplicationSelectorView)obj).Close();
        }
    }
}
