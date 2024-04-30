using System;
using System.Collections.Generic;
using System.Linq;
using Zarp.Core.Datatypes;
using Zarp.GUI.DataTypes;
using static Zarp.Common.Util.PInvoke;
using static Zarp.Common.Util.Window;

namespace Zarp.GUI.Model
{
    internal class ApplicationSelectorModel
    {
        private static string WindowsPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

        private static HashSet<string> IgnoredApps = new HashSet<string>(new string[] {
            Environment.ProcessPath!,
            WindowsPath + @"\explorer.exe",
            WindowsPath + @"\ImmersiveControlPanel\SystemSettings.exe",
            WindowsPath + @"\System32\ApplicationFrameHost.exe",
            WindowsPath + @"\SystemApps\MicrosoftWindows.Client.CBS_cw5n1h2txyewy\TextInputHost.exe"
        });

        private static InstalledApplicationList? ApplicationList;
        private static List<ItemWithIcon<ApplicationInfo>>? Applications;

        public static IEnumerable<ItemWithIcon<ApplicationInfo>> OpenApplications
        {
            get
            {
                if (ApplicationList == null)
                {
                    ApplicationList = new InstalledApplicationList();
                    UpdateInstalledApplications();
                }

                List<ItemWithIcon<ApplicationInfo>> windows = new List<ItemWithIcon<ApplicationInfo>>(16);

                EnumWindows((hWnd, lParam) =>
                {
                    // Ignore invisible windows
                    if (!IsWindowVisible(hWnd))
                    {
                        return true;
                    }

                    string? title = GetWindowTitle(hWnd);

                    // Ignore titleless windows
                    if (string.IsNullOrEmpty(title))
                    {
                        return true;
                    }

                    string? executablePath = GetWindowExecutablePath(hWnd);

                    // Ignore windows with hidden executable path
                    if (executablePath == null || IgnoredApps.Contains(executablePath))
                    {
                        return true;
                    }

                    ApplicationInfo info = new ApplicationInfo(title, executablePath);
                    windows.Add(new ItemWithIcon<ApplicationInfo>(info, info.GetIconAsBitmapSource()));

                    return true;
                }, IntPtr.Zero);

                return windows;
            }
        }

        public static IEnumerable<ItemWithIcon<ApplicationInfo>> InstalledApplications
        {
            get
            {
                if (ApplicationList == null)
                {
                    ApplicationList = new InstalledApplicationList();
                    UpdateInstalledApplications();
                }
                else if (!ApplicationList.Updated)
                {
                    UpdateInstalledApplications();
                }

                return Applications!;
            }
        }

        public static void UpdateInstalledApplications()
        {
            Applications = new List<ItemWithIcon<ApplicationInfo>>(ApplicationList!.Count);

            foreach (ApplicationInfo application in ApplicationList.OrderBy(application => application.Name))
            {
                Applications.Add(new ItemWithIcon<ApplicationInfo>(application, application.GetIconAsBitmapSource()));
            }
        }
    }
}
