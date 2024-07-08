using System;
using System.Diagnostics;
using System.Windows;
using Zarp.Core.App;
using static Zarp.Common.PInvoke.User32;
using static Zarp.Common.Util.Window;

namespace Zarp
{
    internal partial class App : Application
    {
        public App()
        {
            IntPtr handle = IntPtr.Zero;

            EnumWindows((hWnd, lParam) =>
            {
                string title = GetWindowTitle(hWnd) ?? string.Empty;

                if (title == "Untitled - Notepad")
                {
                    handle = hWnd;
                    return false;
                }

                return true;
            }, IntPtr.Zero);

            if (handle != IntPtr.Zero)
            {
                GetWindowThreadProcessId(handle, out uint threadProcessId);
                Process process = Process.GetProcessById((int)threadProcessId);
                process.EnableRaisingEvents = true;
                ProcessManager notepadManager = new ProcessManager(process);
                notepadManager.EnableBlocking();
                notepadManager.EnableLayoutRestore();
                notepadManager.AddWindow(handle);

                process.Exited += (sender, e) =>
                {
                    process.Dispose();
                    notepadManager.Dispose();
                };
            }
        }
    }
}
