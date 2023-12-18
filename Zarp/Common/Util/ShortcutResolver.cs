using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Zarp.Common.Util
{
    public class ShortcutResolver
    {
        const int MAX_PATH = 260;
        const int STGM_READ = 0;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct WIN32_FIND_DATAW
        {
            public uint dwFileAttributes;
            public long ftCreationTime;
            public long ftLastAccessTime;
            public long ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }

        [ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214F9-0000-0000-C000-000000000046")]
        interface IShellLinkW
        {
            void GetPath([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out WIN32_FIND_DATAW pfd);
            void GetArguments([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
        }


        [ComImport(), Guid("00021401-0000-0000-C000-000000000046")]
        public class ShellLink { }

        public static string GetPath(string filename)
        {
            StringBuilder resolvedFilename = new StringBuilder(MAX_PATH);
            WIN32_FIND_DATAW data = new WIN32_FIND_DATAW();

            ShellLink link = new ShellLink();
            ((IPersistFile)link).Load(filename, STGM_READ);
            ((IShellLinkW)link).GetPath(resolvedFilename, resolvedFilename.Capacity, out data);

            return resolvedFilename.ToString();
        }
    }
}
