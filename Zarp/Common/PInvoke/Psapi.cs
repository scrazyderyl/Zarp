using System;
using System.Runtime.InteropServices;

namespace Zarp.Common.PInvoke
{
    internal class Psapi
    {

        [DllImport("psapi.dll", CharSet = CharSet.Unicode)]
        public static extern int GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] char[] lpFilename, int nSize);
    }
}