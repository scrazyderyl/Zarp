using System.IO;
using Zarp.Common.Cache;
using Zarp.Common.Util;
using Zarp.Core.Datatypes;

namespace Zarp.GUI.Model
{
    public class StartMenuCache : DirectoryFileListCache<ApplicationInfo>
    {
        public StartMenuCache(string path) : base(path, "*.lnk", true) { }

        protected override bool TryFetch(string path, out ApplicationInfo data)
        {
            try
            {
                string shortcutName = Path.GetFileName(path);

                // Ignore uninstallers
                if (shortcutName.Contains("Uninstall"))
                {
                    data = default;
                    return false;
                }

                string executablePath = Shortcut.GetPath(Path.GetFullPath(path));

                // Ignore internet links
                if (executablePath.Equals(string.Empty))
                {
                    data = default;
                    return false;
                }

                // Broken shortcut
                if (!File.Exists(executablePath))
                {
                    data = default;
                    return false;
                }

                string extension = Path.GetExtension(executablePath).ToLowerInvariant();

                // Ignore all files except .exe and .msc
                if (!(extension.Equals(".exe") || extension.Equals(".msc")))
                {
                    data = default;
                    return false;
                }

                string name = shortcutName.Substring(0, shortcutName.Length - 4);
                data = new ApplicationInfo(executablePath, name);
            }
            catch
            {
                data = default;
                return false;
            }

            return true;
        }
    }
}
