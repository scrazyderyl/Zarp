using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Navigation;
using Zarp.Core.Datatypes;
using Zarp.GUI.View;
using static Zarp.Common.PInvoke;

namespace Zarp.Core.Service
{
    public class Blocker
    {
        public RulePreset AlwaysAllowed;
        public RulePreset AlwaysBlocked;

        private Dictionary<IntPtr, BlockedOverlayView> BlockedApplicationOverlays;
        private Dictionary<string, RewardPreset> EnabledRewards;
        private bool Enabled;
        private Event? ActiveEvent;

        public Blocker()
        {
            AlwaysAllowed = new RulePreset("Always Allowed");
            AlwaysBlocked = new RulePreset("Always Blocked");

            BlockedApplicationOverlays = new Dictionary<IntPtr, BlockedOverlayView>();
            EnabledRewards = new Dictionary<string, RewardPreset>();
            Enabled = false;
            ActiveEvent = null;
        }

        public static List<IntPtr> GetVisibleWindows()
        {
            List<IntPtr> visibleWindows = new List<IntPtr>();

            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                try
                {
                    if (IsWindowVisible(process.MainWindowHandle) && !IsIconic(process.MainWindowHandle) && !string.IsNullOrEmpty(process.MainWindowTitle))
                    {
                        visibleWindows.Add(process.MainWindowHandle);
                    }
                }
                catch { }
            }

            return visibleWindows;
        }

        public void Enable()
        {
            Enabled = true;
            UpdateAll();
        }

        public void Disable()
        {
            foreach (BlockedOverlayView window in BlockedApplicationOverlays.Values)
            {
                window.Close();
            }

            BlockedApplicationOverlays.Clear();
            Enabled = false;
        }

        public void WindowClosed(IntPtr handle)
        {
            BlockedApplicationOverlays.Remove(handle);
        }

        public void SetActiveEvent(Event newEvent)
        {
            ActiveEvent = newEvent;
            UpdateAll();
        }

        public void ClearActiveEvent()
        {
            ActiveEvent = null;
            UpdateAll();
        }

        public void EnableReward(RewardPreset reward)
        {
            try
            {
                EnabledRewards.Add(reward.Name, reward);
            }
            catch { }
        }

        public void DisableReward(string name)
        {
            EnabledRewards.Remove(name);
        }

        public bool IsRewardEnabled(string name)
        {
            return EnabledRewards.ContainsKey(name);
        }

        public void AddAlwaysAllowedApplications(List<ApplicationInfo> applications)
        {
            foreach (ApplicationInfo application in applications)
            {
                AlwaysBlocked.ApplicationRules.RemoveRule(application.Id);

                try
                {
                    AlwaysAllowed.ApplicationRules.AddRule(application);
                }
                catch { }
            }

            if (Enabled)
            {
                foreach (IntPtr handle in GetVisibleWindows())
                {
                    string? executablePath = GetWindowExecutablePath(handle);

                    if (executablePath == null)
                    {
                        return;
                    }

                    if (AlwaysAllowed.ApplicationRules.Contains(executablePath))
                    {
                        TryUnblockApplication(handle);
                    }
                }
            }
        }

        public void RemoveAlwaysAllowedApplication(ApplicationInfo application)
        {
            AlwaysAllowed.ApplicationRules.RemoveRule(application.Id);

            if (Enabled)
            {
                foreach (IntPtr handle in GetVisibleWindows())
                {
                    string? executablePath = GetWindowExecutablePath(handle);

                    if (executablePath == null || ActiveEvent == null || ActiveEvent.Rules == null)
                    {
                        return;
                    }

                    if (ActiveEvent.Rules.ApplicationRules.IsBlocked(executablePath))
                    {
                        TryBlockApplication(handle);
                    }
                }
            }
        }

        public void AddAlwaysBlockedApplications(List<ApplicationInfo> applications)
        {
            foreach (ApplicationInfo application in applications)
            {
                AlwaysAllowed.ApplicationRules.RemoveRule(application.Id);

                try
                {
                    AlwaysBlocked.ApplicationRules.AddRule(application);
                }
                catch { }
            }

            if (Enabled)
            {
                foreach (IntPtr handle in GetVisibleWindows())
                {
                    string? executablePath = GetWindowExecutablePath(handle);

                    if (executablePath == null)
                    {
                        return;
                    }

                    if (AlwaysBlocked.ApplicationRules.Contains(executablePath))
                    {
                        TryBlockApplication(handle);
                    }
                }
            }
        }

        public void RemoveAlwaysBlockedApplication(ApplicationInfo application)
        {
            AlwaysBlocked.ApplicationRules.RemoveRule(application.Id);

            if (Enabled)
            {
                foreach (IntPtr handle in GetVisibleWindows())
                {
                    string? executablePath = GetWindowExecutablePath(handle);

                    if (executablePath == null || ActiveEvent == null || ActiveEvent.Rules == null)
                    {
                        return;
                    }

                    if (!ActiveEvent.Rules.ApplicationRules.IsBlocked(executablePath))
                    {
                        TryUnblockApplication(handle);
                    }
                }
            }
        }

        public void UpdateAll()
        {
            foreach (IntPtr handle in GetVisibleWindows())
            {
                UpdateApplication(handle);
            }
        }

        public void UpdateApplication(IntPtr handle)
        {
            string? executablePath = GetWindowExecutablePath(handle);

            if (executablePath == null)
            {
                return;
            }

            if (AlwaysAllowed.ApplicationRules.Contains(executablePath))
            {
                TryUnblockApplication(handle);
                return;
            }

            if (AlwaysBlocked.ApplicationRules.Contains(executablePath))
            {
                TryBlockApplication(handle);
                return;
            }

            if (ActiveEvent == null || ActiveEvent.Rules == null)
            {
                return;
            }

            if (ActiveEvent.Rules.ApplicationRules.IsBlocked(executablePath))
            {
                TryBlockApplication(handle);
                return;
            }


            TryUnblockApplication(handle);
        }

        public void TryBlockApplication(IntPtr handle)
        {
            if (!BlockedApplicationOverlays.ContainsKey(handle))
            {
                BlockedApplicationOverlays.Add(handle, new BlockedOverlayView(handle));
            }
        }

        public void TryUnblockApplication(IntPtr handle)
        {
            if (BlockedApplicationOverlays.Remove(handle, out BlockedOverlayView? window))
            {
                window.Close();
            }
        }
    }
}
