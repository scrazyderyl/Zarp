using System.Diagnostics;
using System.Drawing;
using System.IO;
using Zarp.Common.Cache;
using Zarp.Common.Util;
using Zarp.Core.Datatypes;

namespace Zarp.GUI.Model
{
    internal class StartMenuCache : DirectoryFileListCache<ApplicationInfo>
    {
        public StartMenuCache(string path) : base(path, "*.lnk", true) { }

        protected override bool TryFetch(string path, out ApplicationInfo data)
        {
            try
            {
                string shortcutName = Path.GetFileNameWithoutExtension(path);
                string? executablePath = Shortcut.GetPath(path);
                string? arguments = Shortcut.GetArguments(path);

                // Ignore internet links
                if (string.IsNullOrEmpty(executablePath))
                {
                    data = ApplicationInfo.Empty;
                    return false;
                }

                string extension = Path.GetExtension(executablePath).ToLowerInvariant();

                // Ignore all files except executables
                if (extension != ".exe")
                {
                    data = ApplicationInfo.Empty;
                    return false;
                }

                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(executablePath);
                string fileName = Path.GetFileNameWithoutExtension(executablePath);

                data = new ApplicationInfo(shortcutName, fileName, fileVersionInfo.CompanyName);

                if (!ApplicationInfo.Icons.ContainsKey(data))
                {
                    Icon? FileIcon = Icon.ExtractAssociatedIcon(executablePath);
                    ApplicationInfo.Icons.Add(data, FileIcon);
                }
            }
            catch
            {
                data = ApplicationInfo.Empty;
                return false;
            }

            return true;
        }
    }
}
