using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using static Zarp.Core.PInvoke;

namespace Zarp.Core
{
    class GUI
    {
        private static Regex RegistryKeyExcludeMatcher = new Regex(@"\{.+-.+-.+-.+-.+\}");

        public static List<ApplicationInfo> GetInstalledApps()
        {
            List<ApplicationInfo> list = new List<ApplicationInfo>();
            string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

            using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(registry_key))
            {
                if (key == null)
                {
                    return list;
                }

                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    if (RegistryKeyExcludeMatcher.IsMatch(subkey_name))
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
