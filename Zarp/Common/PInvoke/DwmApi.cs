using System;
using System.Runtime.InteropServices;
using static Zarp.Common.PInvoke.User32;

namespace Zarp.Common.PInvoke
{
    internal class DwmApi
    {

        [DllImport("Dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, uint dwAttribute, out RECT pvAttribute, uint cbAttribute);


        [DllImport("Dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, uint dwAttribute, RECT pvAttribute, uint cbAttribute);
    }
}