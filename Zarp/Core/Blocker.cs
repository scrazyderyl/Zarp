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

        private HashSet<string> AlwaysAllowedApplications;
        private HashSet<string> AlwaysBlockedApplications;

        private Event? ActiveEvent;

        IntPtr foregroundChangedEvent;

        public Blocker()
        {
            BlockedApplicationOverlays = new Dictionary<IntPtr, BlockedOverlayView>();
            AlwaysAllowedApplications = new HashSet<string>();
            AlwaysBlockedApplications = new HashSet<string>();
            ActiveEvent = null;

            Enable();
        }

        public void Enable()
        {
            UpdateAll();
            //foregroundChangedEvent = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, new WinEventDelegate(OnForegroundWindowChanged), 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        public void Disable()
        {
            UnhookWinEvent(foregroundChangedEvent);

            foreach (BlockedOverlayView window in BlockedApplicationOverlays.Values)
            {
                window.Close();
            }

            BlockedApplicationOverlays.Clear();
        }

        public void OnForegroundWindowChanged(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint isEventThread, uint dwmsEventTime)
        {
            UpdateApplication(GetForegroundWindow());
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

        public void AddGloballyAllowedApplication(ApplicationInfo application)
        {
            AlwaysBlockedApplications.Remove(application.ExecutablePath);
            AlwaysAllowedApplications.Add(application.ExecutablePath);

            foreach (IntPtr handle in GetVisibleWindows())
            {
                string? executablePath = GetWindowExecutablePath(handle);

                if (executablePath == null)
                {
                    return;
                }

                if (application.ExecutablePath.Equals(executablePath))
                {
                    TryUnblockApplication(handle);
                }
            }
        }

        public void RemoveGloballyAllowedApplication(ApplicationInfo application)
        {
            AlwaysBlockedApplications.Remove(application.ExecutablePath);

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

        public void AddGloballyBlockedApplication(ApplicationInfo application)
        {
            AlwaysAllowedApplications.Remove(application.ExecutablePath);
            AlwaysBlockedApplications.Add(application.ExecutablePath);

            foreach (IntPtr handle in GetVisibleWindows())
            {
                string? executablePath = GetWindowExecutablePath(handle);

                if (executablePath == null)
                {
                    return;
                }

                if (application.ExecutablePath.Equals(executablePath))
                {
                    TryBlockApplication(handle);
                }
            }
        }

        public void RemoveGloballyBlockedApplication(ApplicationInfo application) {
            AlwaysBlockedApplications.Remove(application.ExecutablePath);

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

            if (AlwaysAllowedApplications.Contains(executablePath))
            {
                TryUnblockApplication(handle);
                return;
            }

            if (AlwaysBlockedApplications.Contains(executablePath))
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
