using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using static Zarp.Core.PInvoke;

namespace Zarp.Core
{
    class Util
    {
        private static string[] AlwaysIgnored = new string[] { "Zarp", "Settings", "Program Manager", "Microsoft Text Input Application" };
        private static HashSet<string> IgnoredApps = new HashSet<string>(AlwaysIgnored);

        private static string MuiCacheRegistryKey = @"SOFTWARE\Classes\Local Settings\Software\Microsoft\Windows\Shell\MuiCache";
        private static string MuiMatchString = ".FriendlyAppName";
        private static string StartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + @"\Programs";
        private static string InstalledProgramsRegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        private static Regex InstalledApplicationsExcludeMatcher = new Regex(@"\{.+-.+-.+-.+-.+\}");

        public static List<ApplicationInfo> GetOpenWindows()
        {
            List<ApplicationInfo> windows = new List<ApplicationInfo>();

            EnumWindows((hWnd, lParam) =>
            {
                if (!IsWindowVisible(hWnd))
                {
                    return true;
                }

                string? title = GetWindowTitle(hWnd);
                string? executablePath = GetWindowExecutablePath(hWnd);

                if (String.IsNullOrEmpty(title) || IgnoredApps.Contains(title) || executablePath == null)
                {
                    return true;
                }

                ApplicationInfo app = new ApplicationInfo(executablePath, title);
                windows.Add(app);

                return true;
            }, IntPtr.Zero);

            return windows;
        }

        public static List<IntPtr> GetVisibleWindows()
        {
            List<IntPtr> processes = new List<IntPtr>();

            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd) && !IsIconic(hWnd) && !string.IsNullOrEmpty(GetWindowTitle(hWnd)))
                {
                    processes.Add(hWnd);
                }

                return true;
            }, IntPtr.Zero);

            return processes;
        }

        public static List<ApplicationInfo> GetRecentApplications()
        {
            List<ApplicationInfo> list = new List<ApplicationInfo>();
            HashSet<string> seen = new HashSet<string>();

            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(MuiCacheRegistryKey))
            {
                if (key == null)
                {
                    return list;
                }

                string[] values = key.GetValueNames();

                for (int i = values.Length - 1; i >= 0; i--)
                {
                    string value = values[i];

                    if (value.Length < MuiMatchString.Length || !value.Substring(value.Length - MuiMatchString.Length, MuiMatchString.Length).Equals(MuiMatchString)) {
                        continue;
                    }

                    string executablePath = value.Substring(0, value.Length - MuiMatchString.Length);
                    FileInfo fileInfo = new FileInfo(executablePath);

                    if (fileInfo.Extension != ".exe")
                    {
                        continue;
                    }

                    string applicationName = (string)key.GetValue(value);

                    if (seen.Contains(applicationName))
                    {
                        continue;
                    }

                    seen.Add(applicationName);
                    list.Add(new ApplicationInfo(fileInfo.FullName, applicationName));
                }
            }

            return list;
        }

        // Unfinished
        public static List<ApplicationInfo> GetStartMenuApplications()
        {
            List<ApplicationInfo> list = new List<ApplicationInfo>();
            DirectoryInfo directoryInfo = new DirectoryInfo(StartMenuPath);

            foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.lnk", SearchOption.AllDirectories))
            {
                if (fileInfo.Name.Contains("Uninstall"))
                {
                    continue;
                }

                string name = fileInfo.Name.Substring(0, fileInfo.Name.Length - 4);
                string executablePath = fileInfo.FullName;

                list.Add(new ApplicationInfo(executablePath, name));
            }

            list.Sort();
            return list;
        }

        public static List<ApplicationInfo> GetInstalledApps()
        {
            List<ApplicationInfo> list = new List<ApplicationInfo>();

            using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(InstalledProgramsRegistryKey))
            {
                if (key == null)
                {
                    return list;
                }

                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    if (InstalledApplicationsExcludeMatcher.IsMatch(subkey_name))
                    {
                        continue;
                    }

                    using (RegistryKey? subkey = key.OpenSubKey(subkey_name))
                    {
                        if (subkey == null)
                        {
                            continue;
                        }

                        String? displayName = (String?)subkey.GetValue("DisplayName");

                        if (displayName == null)
                        {
                            continue;
                        }

                        String? iconLocation = (String?)subkey.GetValue("DisplayIcon");
                        int iconIndex = 0;

                        // Parse icon location
                        if (iconLocation != null)
                        {
                            // Icon path
                            string[] tokens = iconLocation.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            iconLocation = tokens[0].Trim('"');

                            // Has icon index
                            if (tokens.Length == 2)
                            {
                                try
                                {
                                    iconIndex = int.Parse(tokens[1]);
                                }
                                catch
                                {

                                }
                            }
                        }

                        list.Add(new ApplicationInfo("", displayName, iconLocation, iconIndex));
                    }
                }
            }

            return list;
        }

        static Bitmap? GetIconAsBitmap(String info)
        {
            string[] tokens = info.Split(",");
            string fileName = info.Trim('"');

            // Generate bitmap from .ico
            if (tokens.Length == 1)
            {
                return GetIconFromIco(fileName);
            }

            int index = int.Parse(tokens[1]);

            // Extract resource icon
            if (index < 0)
            {
                return GetResourceIcon(index);
            }

            // Extract icon form exe
            return GetIconFromExe(fileName, index);
        }

        static Bitmap? GetIconFromIco(String fileName)
        {
            return null;
        }

        static Bitmap? GetIconFromExe(String fileName, int index)
        {
            IntPtr hIcon = ExtractIcon(IntPtr.Zero, fileName, index);

            if (hIcon == IntPtr.Zero)
            {
                return null;
            }

            Icon icon = Icon.FromHandle(hIcon);
            return icon.ToBitmap();
        }

        static Bitmap? GetResourceIcon(int index)
        {
            IntPtr hIcon = ExtractIcon(IntPtr.Zero, @"%SystemRoot%\system32\imageres.dll", index);
            Icon icon = Icon.FromHandle(hIcon);
            return icon.ToBitmap();
        }
    }
}
