using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using Zarp.Core.Datatypes;
using Zarp.GUI.DataTypes;
using static Zarp.Common.Util.PInvoke;

namespace Zarp.GUI.Model
{
    internal class ApplicationSelectorModel
    {
        private static string WindowsPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

        private static string[] IgnoredAppPaths = new string[] {
            Environment.ProcessPath!,
            WindowsPath + @"\explorer.exe",
            WindowsPath + @"\ImmersiveControlPanel\SystemSettings.exe",
            WindowsPath + @"\System32\ApplicationFrameHost.exe",
            WindowsPath + @"\SystemApps\MicrosoftWindows.Client.CBS_cw5n1h2txyewy\TextInputHost.exe"
        };
        private static HashSet<string> IgnoredApps = new HashSet<string>(IgnoredAppPaths);

        private static ApplicationList? ApplicationList;
        private static ApplicationIconCache IconCache = new ApplicationIconCache();
        private static List<ItemWithIcon<ApplicationInfo>>? Applications;

        public static IEnumerable<ItemWithIcon<ApplicationInfo>> OpenApplications
        {
            get
            {
                if (ApplicationList == null)
                {
                    ApplicationList = new ApplicationList();
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

                    IconCache.Get(executablePath, out BitmapSource? icon);
                    windows.Add(new ItemWithIcon<ApplicationInfo>(new ApplicationInfo(executablePath, title), icon));

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
                    ApplicationList = new ApplicationList();
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
                IconCache.Get(application.ExecutablePath, out BitmapSource? icon);
                Applications.Add(new ItemWithIcon<ApplicationInfo>(application, icon));
            }
        }

        public static BitmapSource? GetExecutableIcon(string path)
        {
            IconCache.Get(path, out BitmapSource? data);

            return data;
        }
    }
}
