using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Zarp.Common.Util
{
    internal class Shortcut
    {
        private const int INFOTIPSIZE = 1024;

        private const uint S_OK = 0x00000000;
        private const uint S_FALSE = 0x00000001;

        private const int MAX_PATH = 260;

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

        [Flags()]
        enum STGM_FLAGS
        {
            STGM_READ = 0x00000000,
            STGM_WRITE = 0x00000001,
            STGM_READWRITE = 0x00000002,
            STGM_SHARE_DENY_NONE = 0x00000040,
            STGM_SHARE_DENY_READ = 0x00000030,
            STGM_SHARE_DENY_WRITE = 0x00000020,
            STGM_SHARE_EXCLUSIVE = 0x00000010,
            STGM_PRIORITY = 0x00040000,
            STGM_CREATE = 0x00001000,
            STGM_CONVERT = 0x00020000,
            STGM_FAILIFTHERE = 0x00000000,
            STGM_DIRECT = 0x00000000,
            STGM_TRANSACTED = 0x00010000,
            STGM_NOSCRATCH = 0x00100000,
            STGM_NOSNAPSHOT = 0x00200000,
            STGM_SIMPLE = 0x08000000,
            STGM_DIRECT_SWMR = 0x00400000,
            STGM_DELETEONRELEASE = 0x04000000
        }

        [Flags()]
        enum SLGP_FLAGS
        {
            SLGP_SHORTPATH = 0x1,
            SLGP_UNCPRIORITY = 0x2,
            SLGP_RAWPATH = 0x4
        }

        [Flags()]
        enum SLR_FLAGS
        {
            SLR_NO_UI = 0x0001,
            SLR_ANY_MATCH = 0x0002,
            SLR_UPDATE = 0x0004,
            SLR_NOUPDATE = 0x0008,
            SLR_NOSEARCH = 0x0010,
            SLR_NOTRACK = 0x0020,
            SLR_NOLINKINFO = 0x0040,
            SLR_INVOKE_MSI = 0x0080,
            SLR_NO_UI_WITH_MSG_PUMP = 0x0101,
            SLR_OFFER_DELETE_WITHOUT_FILE = 0x0200,
            SLR_KNOWNFOLDER = 0x0400,
            SLR_MACHINE_IN_LOCAL_TARGET = 0x0800,
            SLR_UPDATE_MACHINE_AND_SID = 0x1000
        }

        [ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214F9-0000-0000-C000-000000000046")]
        interface IShellLinkW
        {
            uint GetPath([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cch, out WIN32_FIND_DATAW pfd, SLGP_FLAGS fFlags);
            uint GetIDList(out IntPtr ppidl);
            uint SetIDList(IntPtr pidl);
            uint GetDescription([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cch);
            uint SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            uint GetWorkingDirectory([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cch);
            uint SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
            uint GetArguments([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cch);
            uint SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
            uint GetHotkey(out short pwHotkey);
            uint SetHotkey(short wHotkey);
            uint GetShowCmd(out int piShowCmd);
            uint SetShowCmd(int iShowCmd);
            uint GetIconLocation([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath,
                int cchIconPath, out int piIcon);
            uint SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
            uint SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
            uint Resolve(IntPtr hwnd, SLR_FLAGS fFlags);
            uint SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);

        }


        [ComImport(), Guid("00021401-0000-0000-C000-000000000046")]
        public class ShellLink { }

        public static string? GetPath(string filename)
        {
            StringBuilder resolvedFilename = new StringBuilder(MAX_PATH);
            ShellLink link = new ShellLink();
            ((IPersistFile)link).Load(filename, (int)STGM_FLAGS.STGM_READ);
            uint result = ((IShellLinkW)link).GetPath(resolvedFilename, MAX_PATH, out _, 0);

            if (result == S_OK)
            {
                return resolvedFilename.ToString();
            }
            else if (result == S_FALSE)
            {
                return string.Empty;
            }

            return null;
        }

        public static string? GetArguments(string filename)
        {
            StringBuilder arguments = new StringBuilder(INFOTIPSIZE);
            ShellLink link = new ShellLink();
            ((IPersistFile)link).Load(filename, (int)STGM_FLAGS.STGM_READ);
            uint result = ((IShellLinkW)link).GetArguments(arguments, INFOTIPSIZE);

            if (result == S_OK)
            {
                return arguments.ToString();
            }

            return null;
        }
    }
}
