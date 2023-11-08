using System;
using System.Collections.Generic;
using Zarp.View;
using static Zarp.Core.PInvoke;
using static Zarp.Core.Util;

namespace Zarp.Core
{
    public class Blocker
    {
        private Dictionary<IntPtr, BlockedOverlayView> BlockedApplicationOverlays;

        private Dictionary<string, ApplicationInfo> AlwaysAllowedApplications;
        private Dictionary<string, ApplicationInfo> AlwaysBlockedApplications;

        private bool Enabled;
        private Event? ActiveEvent;

        public Blocker()
        {
            BlockedApplicationOverlays = new Dictionary<IntPtr, BlockedOverlayView>();
            AlwaysAllowedApplications = new Dictionary<string, ApplicationInfo>();
            AlwaysBlockedApplications = new Dictionary<string, ApplicationInfo>();
            Enabled = false;
            ActiveEvent = null;
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

        public void AddAlwaysAllowedApplications(List<ApplicationInfo> applications)
        {
            foreach (ApplicationInfo application in applications)
            {
                AlwaysBlockedApplications.Remove(application.ExecutablePath);

                try
                {
                    AlwaysAllowedApplications.Add(application.ExecutablePath, application);
                } catch { }
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

                    if (AlwaysBlockedApplications.ContainsKey(executablePath))
                    {
                        TryUnblockApplication(handle);
                    }
                }
            }
        }

        public void RemoveAlwaysAllowedApplication(ApplicationInfo application)
        {
            AlwaysBlockedApplications.Remove(application.ExecutablePath);

            if (Enabled)
            {
                foreach (IntPtr handle in GetVisibleWindows())
                {
                    string? executablePath = GetWindowExecutablePath(handle);

                    if (executablePath == null || ActiveEvent == null)
                    {
                        return;
                    }

                    if (ActiveEvent.IsApplicationBlocked(executablePath))
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
                AlwaysAllowedApplications.Remove(application.ExecutablePath);

                try
                {
                    AlwaysBlockedApplications.Add(application.ExecutablePath, application);
                } catch { }
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

                    if (AlwaysBlockedApplications.ContainsKey(executablePath))
                    {
                        TryBlockApplication(handle);
                    }
                }
            }
        }

        public void RemoveAlwaysBlockedApplication(ApplicationInfo application)
        {
            AlwaysBlockedApplications.Remove(application.ExecutablePath);

            if (Enabled)
            {
                foreach (IntPtr handle in GetVisibleWindows())
                {
                    string? executablePath = GetWindowExecutablePath(handle);

                    if (executablePath == null || ActiveEvent == null)
                    {
                        return;
                    }

                    if (!ActiveEvent.IsApplicationBlocked(executablePath))
                    {
                        TryUnblockApplication(handle);
                    }
                }
            }
        }

        public IEnumerable<ApplicationInfo> GetAlwaysAllowedApplications()
        {
            return AlwaysAllowedApplications.Values;
        }

        public IEnumerable<ApplicationInfo> GetAlwaysBlockedApplications()
        {
            return AlwaysBlockedApplications.Values;
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

            if (AlwaysAllowedApplications.ContainsKey(executablePath))
            {
                TryUnblockApplication(handle);
                return;
            }

            if (AlwaysBlockedApplications.ContainsKey(executablePath))
            {
                TryBlockApplication(handle);
                return;
            }

            if (ActiveEvent == null)
            {
                return;
            }

            if (ActiveEvent.IsApplicationBlocked(executablePath))
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
