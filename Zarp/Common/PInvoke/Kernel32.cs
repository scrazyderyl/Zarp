using System;
using System.Runtime.InteropServices;

namespace Zarp.Common.PInvoke
{
    internal class Kernel32
    {

        public static int PROCESS_QUERY_INFORMATION = 0x0400;
        public static int PROCESS_VM_READ = 0x0010;
        public static int MAX_PATH = 260;

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, uint dwProcessId);
    }
}