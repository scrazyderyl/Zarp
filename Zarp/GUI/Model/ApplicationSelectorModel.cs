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

        private static string[] _IgnoredAppPaths = new string[] {
            System.Windows.Forms.Application.ExecutablePath,
            WindowsPath + @"\explorer.exe",
            WindowsPath + @"\ImmersiveControlPanel\SystemSettings.exe",
            WindowsPath + @"\System32\ApplicationFrameHost.exe",
            WindowsPath + @"\SystemApps\MicrosoftWindows.Client.CBS_cw5n1h2txyewy\TextInputHost.exe"
        };
        private static HashSet<string> _IgnoredApps = new HashSet<string>(_IgnoredAppPaths);

        private static ApplicationList? _ApplicationList;
        private static ApplicationIconCache _IconCache = new ApplicationIconCache();
        private static List<ItemWithIcon<ApplicationInfo>>? _Applications;

        public static IEnumerable<ItemWithIcon<ApplicationInfo>> OpenApplications
        {
            get
            {
                if (_ApplicationList == null)
                {
                    _ApplicationList = new ApplicationList();
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
                    if (executablePath == null || _IgnoredApps.Contains(executablePath))
                    {
                        return true;
                    }

                    _IconCache.Get(executablePath, out BitmapSource? icon);
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
                if (_ApplicationList == null)
                {
                    _ApplicationList = new ApplicationList();
                    UpdateInstalledApplications();
                }
                else if (!_ApplicationList.Updated)
                {
                    UpdateInstalledApplications();
                }

                return _Applications!;
            }
        }

        public static void UpdateInstalledApplications()
        {
            _Applications = new List<ItemWithIcon<ApplicationInfo>>(_ApplicationList!.Count);

            foreach (ApplicationInfo application in _ApplicationList.OrderBy(application => application.Name))
            {
                _IconCache.Get(application.ExecutablePath, out BitmapSource? icon);
                _Applications.Add(new ItemWithIcon<ApplicationInfo>(application, icon));
            }
        }

        public static BitmapSource? GetExecutableIcon(string path)
        {
            _IconCache.Get(path, out BitmapSource? data);

            return data;
        }
    }
}
