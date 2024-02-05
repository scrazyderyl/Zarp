using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using Zarp.Core.Datatypes;
using Zarp.GUI.View;
using static Zarp.Common.Util.PInvoke;

namespace Zarp.Core.App
{
    internal class Blocker
    {
        public RulePreset AlwaysAllowed;
        public RulePreset AlwaysBlocked;

        private Dictionary<IntPtr, BlockedOverlayView> BlockedApplicationOverlays;
        private Dictionary<string, RewardPreset> EnabledRewards;
        private Event? ActiveEvent;
        private bool Enabled;

        private WinEventDelegate ForegroundEventHandler;
        private IntPtr ForegroundEventHook;

        public Blocker()
        {
            AlwaysAllowed = new RulePreset();
            AlwaysBlocked = new RulePreset();

            BlockedApplicationOverlays = new Dictionary<IntPtr, BlockedOverlayView>();
            EnabledRewards = new Dictionary<string, RewardPreset>();
            ActiveEvent = null;
            Enabled = false;

            ForegroundEventHandler = OnForegroundChanged;
        }

        public void Enable()
        {
            Enabled = true;
            ForegroundEventHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, ForegroundEventHandler, 0, 0, WINEVENT_OUTOFCONTEXT);
            UpdateAll();
        }

        public void Disable()
        {
            Enabled = false;

            foreach (BlockedOverlayView window in BlockedApplicationOverlays.Values)
            {
                window.Close();
            }

            BlockedApplicationOverlays.Clear();
            UnhookWinEvent(ForegroundEventHook);
        }

        private void OnForegroundChanged(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint isEventThread, uint dwmsEventTime)
        {
            if (idObject != OBJID_WINDOW)
            {
                return;
            }

            if (BlockedApplicationOverlays.TryGetValue(hwnd, out BlockedOverlayView? overlay))
            {
                overlay.Activate();
            }
            else if (IsApplicationBlocked(hwnd))
            {
                TryBlockApplication(hwnd);
            }
        }

        internal void WindowClosed(IntPtr handle)
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
            EnabledRewards.TryAdd(reward.Name, reward);
        }

        public void DisableReward(string name)
        {
            EnabledRewards.Remove(name);
        }

        public bool IsRewardEnabled(string name)
        {
            return EnabledRewards.ContainsKey(name);
        }

        public void AddAlwaysAllowed(IEnumerable<ApplicationInfo> applications)
        {
            foreach (ApplicationInfo application in applications)
            {
                AlwaysBlocked.ApplicationRules.Remove(application.Id);
                AlwaysAllowed.ApplicationRules.Add(application);
            }

            if (Enabled)
            {
                UpdateAll();
            }
        }

        public void RemoveAlwaysAllowed(ApplicationInfo application)
        {
            AlwaysAllowed.ApplicationRules.Remove(application.Id);

            if (Enabled)
            {
                UpdateAll();
            }
        }

        public void AddAlwaysBlocked(IEnumerable<ApplicationInfo> applications)
        {
            foreach (ApplicationInfo application in applications)
            {
                AlwaysAllowed.ApplicationRules.Remove(application.Id);
                AlwaysBlocked.ApplicationRules.Add(application);
            }

            if (Enabled)
            {
                UpdateAll();
            }
        }

        public void RemoveAlwaysBlocked(ApplicationInfo application)
        {
            AlwaysBlocked.ApplicationRules.Remove(application.Id);

            if (Enabled)
            {
                UpdateAll();
            }
        }

        public bool IsApplicationBlocked(IntPtr handle)
        {
            string? executablePath = GetWindowExecutablePath(handle);

            if (executablePath == null)
            {
                return false;
            }
            else if (AlwaysAllowed.ApplicationRules.Contains(executablePath))
            {
                return false;
            }
            else if (AlwaysBlocked.ApplicationRules.Contains(executablePath))
            {
                return true;
            }
            else if (ActiveEvent == null)
            {
                return false;
            }
            else if (ActiveEvent.Type == EventType.OfflineBreak)
            {
                return true;
            }
            else if (ActiveEvent.Rules != null && ActiveEvent.Rules.ApplicationRules.IsBlocked(executablePath))
            {
                return true;
            }

            return false;
        }

        public void UpdateAll()
        {
            foreach (IntPtr handle in GetVisibleWindows())
            {
                if (IsApplicationBlocked(handle))
                {
                    TryBlockApplication(handle);
                }
                else
                {
                    TryUnblockApplication(handle);
                }
            }

            if (Application.Current.MainWindow.IsVisible)
            {
                SetWindowPos(new WindowInteropHelper(Application.Current.MainWindow).Handle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            }
        }

        private void TryBlockApplication(IntPtr handle)
        {
            if (!BlockedApplicationOverlays.ContainsKey(handle))
            {
                BlockedApplicationOverlays.Add(handle, new BlockedOverlayView(handle));
            }
        }

        private void TryUnblockApplication(IntPtr handle)
        {
            if (BlockedApplicationOverlays.Remove(handle, out BlockedOverlayView? overlay))
            {
                overlay.Close();
            }
        }

        public static IEnumerable<IntPtr> GetVisibleWindows()
        {
            List<IntPtr> visibleWindows = new List<IntPtr>(16);

            EnumWindows((hWnd, lParam) =>
            {
                // Ignore invisible and minimized windows
                if (!IsWindowVisible(hWnd) || IsIconic(hWnd))
                {
                    return true;
                }

                string? title = GetWindowTitle(hWnd);

                // Ignore titleless windows
                if (string.IsNullOrEmpty(title))
                {
                    return true;
                }

                visibleWindows.Add(hWnd);

                return true;
            }, IntPtr.Zero);

            return visibleWindows;
        }
    }
}
