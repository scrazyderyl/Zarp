using System.IO;
using Zarp.Common.Cache;
using Zarp.Common.Util;
using Zarp.Core.Datatypes;

namespace Zarp.GUI.Util
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
                    data = default(ApplicationInfo);
                    return false;
                }

                string executablePath = ShortcutResolver.GetPath(Path.GetFullPath(path));

                // Ignore internet links
                if (executablePath.Equals(string.Empty))
                {
                    data = default(ApplicationInfo);
                    return false;
                }

                // Broken shortcut
                if (!File.Exists(executablePath))
                {
                    data = default(ApplicationInfo);
                    return false;
                }

                string extension = Path.GetExtension(executablePath).ToLowerInvariant();

                // Ignore all files except .exe and .msc
                if (!(extension.Equals(".exe") || extension.Equals(".msc")))
                {
                    data = default(ApplicationInfo);
                    return false;
                }

                string name = shortcutName.Substring(0, shortcutName.Length - 4);
                data = new ApplicationInfo(executablePath, name);
            }
            catch
            {
                data = default(ApplicationInfo);
                return false;
            }

            return true;
        }
    }
}
