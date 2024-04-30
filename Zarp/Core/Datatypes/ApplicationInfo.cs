using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Zarp.Core.Datatypes
{
    internal class ApplicationInfo
    {
        internal static ApplicationInfo Empty = new ApplicationInfo(string.Empty, string.Empty, null);
        internal static Dictionary<ApplicationInfo, Icon?> Icons = new Dictionary<ApplicationInfo, Icon?>();

        public string Name;
        public string FileName;
        public string? CompanyName;

        static ApplicationInfo()
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            foreach (Icon? icon in Icons.Values)
            {
                icon?.Dispose();
            }
        }

        public ApplicationInfo(string path)
        {
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(path);

            if (fileVersionInfo.ProductName == null)
            {
                throw new Exception();
            }

            Name = fileVersionInfo.ProductName;
            FileName = Path.GetFileNameWithoutExtension(path);
            CompanyName = fileVersionInfo.CompanyName;

            if (!Icons.ContainsKey(this))
            {
                Icon? icon = Icon.ExtractAssociatedIcon(path);
                Icons.Add(this, icon);
            }
        }

        public ApplicationInfo(string name, string path)
        {
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(path);

            if (fileVersionInfo.ProductName == null)
            {
                throw new Exception();
            }

            Name = name;
            FileName = Path.GetFileNameWithoutExtension(path);
            CompanyName = fileVersionInfo.CompanyName;

            if (!Icons.ContainsKey(this))
            {
                Icon? icon = Icon.ExtractAssociatedIcon(path);
                Icons.Add(this, icon);
            }
        }

        public ApplicationInfo(string name, string fileName, string? companyName)
        {
            Name = name;
            FileName = fileName;
            CompanyName = companyName;
        }

        public BitmapSource? GetIconAsBitmapSource()
        {
            Icons.TryGetValue(this, out Icon? icon);

            return icon == null ? null : Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            ApplicationInfo other = (ApplicationInfo)obj;

            return other.FileName == FileName && other.CompanyName == CompanyName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FileName, CompanyName);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
