using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Automation;
using Zarp.Core.Datatypes;
using static Zarp.Common.PInvoke.User32;

namespace Zarp.Core.App
{
    internal class DesktopManager
    {
        private static Dictionary<ApplicationInfo, HashSet<int>> ApplicationInfoCache;
        private static Dictionary<int, ProcessManager> ManagedProcesses;

        static DesktopManager()
        {
            ApplicationInfoCache = new Dictionary<ApplicationInfo, HashSet<int>>();
            ManagedProcesses = new Dictionary<int, ProcessManager>();

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            Automation.RemoveAutomationEventHandler(WindowPattern.WindowOpenedEvent, AutomationElement.RootElement, OnWindowOpened);

            foreach (ProcessManager processManager in ManagedProcesses.Values)
            {
                processManager.Dispose();
            }
        }

        private static void Process_Exited(object? sender, EventArgs e)
        {
            Process process = (sender as Process)!;

            RemoveProcess(process.Id);
        }

        private static void OnWindowOpened(object sender, AutomationEventArgs e)
        {
            AutomationElement window = (AutomationElement)sender;

            AddWindow(new IntPtr(window.Current.NativeWindowHandle));
        }

        public static void Enable()
        {
            Automation.AddAutomationEventHandler(WindowPattern.WindowOpenedEvent, AutomationElement.RootElement, TreeScope.Children, OnWindowOpened);

            EnumWindows((hwnd, lParam) =>
            {
                AddWindow(hwnd);

                return true;
            }, IntPtr.Zero);
        }

        public static void Disable()
        {
            Automation.RemoveAutomationEventHandler(WindowPattern.WindowOpenedEvent, AutomationElement.RootElement, OnWindowOpened);

            foreach (ProcessManager processManager in ManagedProcesses.Values)
            {
                processManager.Dispose();
            }

            ManagedProcesses.Clear();
        }

        public static void EnableLayoutRestore()
        {
            foreach (ProcessManager processManager in ManagedProcesses.Values)
            {
                processManager.EnableLayoutRestore();
            }
        }

        public static void DisableLayoutRestore()
        {
            foreach (ProcessManager processManager in ManagedProcesses.Values)
            {
                processManager.DisableLayoutRestore();
            }
        }

        public static bool StartProcess(ApplicationStartInfo applicationStartInfo)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = applicationStartInfo.FileName,
                Arguments = applicationStartInfo.Arguments
            };

            Process process = new Process()
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            try
            {
                if (!process.Start())
                {
                    return false;
                };
            }
            catch
            {
                return false;
            }

            ProcessManager processManager = new ProcessManager(process);
            process.Exited += Process_Exited;
            ManagedProcesses.Add(process.Id, processManager);

            return true;
        }

        private static void AddWindow(IntPtr hWnd)
        {
            GetWindowThreadProcessId(hWnd, out uint lpdwProcessId);

            if (lpdwProcessId == 0)
            {
                return;
            }

            int processId = (int)lpdwProcessId;

            if (!ManagedProcesses.TryGetValue(processId, out ProcessManager? processManager))
            {
                try
                {
                    processManager = AddProcess(processId);
                }
                catch
                {
                    return;
                }
            }

            processManager.AddWindow(hWnd);
        }

        private static ProcessManager AddProcess(int processId)
        {
            Process process = Process.GetProcessById(processId);
            process.EnableRaisingEvents = true;

            ProcessManager processManager = new ProcessManager(process);
            ManagedProcesses.Add(process.Id, processManager);
            process.Exited += Process_Exited;

            return processManager;
        }

        private static void RemoveProcess(int processId)
        {
            if (ManagedProcesses.TryGetValue(processId, out ProcessManager? processManager))
            {
                processManager.Dispose();
                ManagedProcesses.Remove(processId);
            }
        }

        private static void Refresh()
        {
            foreach (KeyValuePair<int, ProcessManager> process in ManagedProcesses)
            {
                if (Session.IsApplicationBlocked(process.Value.ApplicationInfo!))
                {
                    RemoveProcess(process.Key);
                }
            }

            EnumWindows((hWnd, lParam) =>
            {
                AddWindow(hWnd);

                return true;
            }, IntPtr.Zero);
        }
    }
}
