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

namespace Zarp.ViewModel
{

    internal class ApplicationSelectorViewModel
    {
        public RelayCommand SelectExecutableCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand DoneCommand { get; set; }

        public ObservableCollection<string> OpenApplications { get; set; }
        public ObservableCollection<string> InstalledApplications { get; set; }
        public ObservableCollection<string> UserSpecifiedApplications { get; set; }

        public List<string> SelectedApplications;

        public ApplicationSelectorViewModel()
        {
            OpenApplications = new ObservableCollection<string>();
            InstalledApplications = new ObservableCollection<string>();
            UserSpecifiedApplications = new ObservableCollection<string>();

            SelectedApplications = new List<string>();

            foreach (ApplicationInfo app in GetOpenWindows())
            {
                OpenApplications.Add(app.Name);
            }

            foreach (ApplicationInfo app in GetStartMenuApplications())
            {
                InstalledApplications.Add(app.Name);
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

            foreach (string item in fileDialog.FileNames)
            {
                if (UserSpecifiedApplications.Contains(item))
                {
                    continue;
                }

                UserSpecifiedApplications.Add(item);
            }
        }

        private void Cancel(object? obj)
        {

        }

        private void Done(object? obj)
        {
            
        }
    }
}
