using Microsoft.Win32;
using Shell32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Zarp.Core.Datatypes;
using Zarp.GUI.Util;
using static Zarp.Common.PInvoke;
using Zarp.Common;

namespace Zarp.GUI.Model
{
    class ApplicationSelectorModel
    {
        private static string WindowsPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        private static string CommonStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + @"\Programs";
        private static string UserStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\Programs";

        private static HashSet<string> _IgnoredApps = new HashSet<string>(new string[] { System.Windows.Forms.Application.ExecutablePath, WindowsPath + @"\ImmersiveControlPanel\SystemSettings.exe", WindowsPath + @"\System32\ApplicationFrameHost.exe", WindowsPath + @"\SystemApps\MicrosoftWindows.Client.CBS_cw5n1h2txyewy\TextInputHost.exe" });

        private static Dictionary<string, ApplicationInfo> ApplicationLookup = new Dictionary<string, ApplicationInfo>();

        public static IEnumerable<ApplicationInfo> GetOpenApplications()
        {
            List<ApplicationInfo> applications = new List<ApplicationInfo>();
            Process[] Processes = Process.GetProcesses();

            foreach (Process process in Processes)
            {
                string? executablePath = GetWindowExecutablePath(process.MainWindowHandle);

                // Ignores titleless windows, .msc, Zarp, and invisible windows
                if (executablePath == null || string.IsNullOrEmpty(process.MainWindowTitle) || _IgnoredApps.Contains(executablePath))
                {
                    continue;
                }

                if (ApplicationLookup.TryGetValue(executablePath, out var applicationInfo))
                {
                    applications.Add(applicationInfo);
                }
                else
                {
                    applications.Add(new ApplicationInfo(executablePath, process.MainWindowTitle));
                }
            }

            return applications;
        }

        public static IEnumerable<ApplicationInfo> GetInstalledApplications()
        {
            return GetStartMenuApplications(CommonStartMenuPath).Union(GetStartMenuApplications(UserStartMenuPath)).OrderBy(application => application.Name);
        }

        public static IEnumerable<ApplicationInfo> GetStartMenuApplications(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            foreach (FileInfo fileInfo in directoryInfo.EnumerateFiles("*.lnk", SearchOption.AllDirectories))
            {
                // Ignore uninstallers
                if (fileInfo.Name.Contains("Uninstall"))
                {
                    continue;
                }

                string executablePath = ShortcutResolver.GetPath(fileInfo.FullName);

                // Ignore internet links
                if (executablePath.Equals(string.Empty))
                {
                    continue;
                }

                FileInfo executableFileInfo = new FileInfo(executablePath);

                // Broken shortcut
                if (!executableFileInfo.Exists)
                {
                    continue;
                }

                string extension = executableFileInfo.Extension.ToLowerInvariant();

                // Ignore all files except .exe and .msc
                if (!(extension.Equals(".exe") || extension.Equals(".msc")))
                {
                    continue;
                }

                string name = Path.GetFileNameWithoutExtension(fileInfo.Name);
                ApplicationInfo applicationInfo = new ApplicationInfo(executablePath, name);
                ApplicationLookup.TryAdd(executablePath, applicationInfo);

                yield return applicationInfo;
            }
        }
    }
}
