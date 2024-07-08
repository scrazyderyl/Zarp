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

                // Ignore uninstallers
                if (shortcutName.Contains("Uninstall"))
                {
                    data = ApplicationInfo.Empty;
                    return false;
                }

                string? executablePath = Shortcut.GetPath(path);

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

                string fileName = Path.GetFileNameWithoutExtension(executablePath);
                string arguments = Shortcut.GetArguments(path) ?? string.Empty;

                data = new ApplicationStartInfo(shortcutName, executablePath, arguments);
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
