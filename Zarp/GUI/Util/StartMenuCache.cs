using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Zarp.Common;
using Zarp.Core.Datatypes;

namespace Zarp.GUI.Util
{
    public class StartMenuCache : DirectoryListCache<ApplicationInfo>
    {
        public StartMenuCache(string path) : base(path) { }

        protected override IEnumerable<FileInfo> GetFiles(DirectoryInfo directory)
        {
            return directory.EnumerateFiles("*.lnk", SearchOption.AllDirectories);
        }

        protected override bool TryFetch(FileInfo file, out ApplicationInfo data)
        {
            // Ignore uninstallers
            if (file.Name.Contains("Uninstall"))
            {
                data = default(ApplicationInfo);
                return false;
            }

            string executablePath = ShortcutResolver.GetPath(file.FullName);

            // Ignore internet links
            if (executablePath.Equals(string.Empty))
            {
                data = default(ApplicationInfo);
                return false;
            }

            FileInfo executableFileInfo = new FileInfo(executablePath);

            // Broken shortcut
            if (!executableFileInfo.Exists)
            {
                data = default(ApplicationInfo);
                return false;
            }

            string extension = executableFileInfo.Extension.ToLowerInvariant();

            // Ignore all files except .exe and .msc
            if (!(extension.Equals(".exe") || extension.Equals(".msc")))
            {
                data = default(ApplicationInfo);
                return false;
            }

            string name = file.Name.Substring(0, file.Name.Length - 4);
            data = new ApplicationInfo(executablePath, name);

            return true;
        }
    }
}
