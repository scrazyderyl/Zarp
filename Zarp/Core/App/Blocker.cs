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
        internal RulePreset _AlwaysAllowed;
        internal RulePreset _AlwaysBlocked;

        private Dictionary<IntPtr, BlockedOverlayView> _BlockedApplicationOverlays;
        private Dictionary<string, RewardPreset> _EnabledRewards;
        private Event? _ActiveEvent;
        private bool _Enabled;

        private WinEventDelegate _ForegroundEventHandler;
        private IntPtr _ForegroundEventHook;

        public Blocker()
        {
            _AlwaysAllowed = new RulePreset();
            _AlwaysBlocked = new RulePreset();

            _BlockedApplicationOverlays = new Dictionary<IntPtr, BlockedOverlayView>();
            _EnabledRewards = new Dictionary<string, RewardPreset>();
            _ActiveEvent = null;
            _Enabled = false;

            _ForegroundEventHandler = OnForegroundChanged;
        }

        public void Enable()
        {
            _Enabled = true;
            _ForegroundEventHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _ForegroundEventHandler, 0, 0, WINEVENT_OUTOFCONTEXT);
            UpdateAll();
        }

        public void Disable()
        {
            _Enabled = false;

            foreach (BlockedOverlayView window in _BlockedApplicationOverlays.Values)
            {
                window.Close();
            }

            _BlockedApplicationOverlays.Clear();
            UnhookWinEvent(_ForegroundEventHook);
        }

        private void OnForegroundChanged(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint isEventThread, uint dwmsEventTime)
        {
            if (idObject != OBJID_WINDOW)
            {
                return;
            }

            if (_BlockedApplicationOverlays.TryGetValue(hwnd, out BlockedOverlayView? overlay))
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
            _BlockedApplicationOverlays.Remove(handle);
        }

        public void SetActiveEvent(Event newEvent)
        {
            _ActiveEvent = newEvent;
            UpdateAll();
        }

        public void ClearActiveEvent()
        {
            _ActiveEvent = null;
            UpdateAll();
        }

        public void EnableReward(RewardPreset reward)
        {
            _EnabledRewards.TryAdd(reward.Name, reward);
        }

        public void DisableReward(string name)
        {
            _EnabledRewards.Remove(name);
        }

        public bool IsRewardEnabled(string name)
        {
            return _EnabledRewards.ContainsKey(name);
        }

        public void AddAlwaysAllowed(IEnumerable<ApplicationInfo> applications)
        {
            foreach (ApplicationInfo application in applications)
            {
                _AlwaysBlocked.ApplicationRules.Remove(application.Id);
                _AlwaysAllowed.ApplicationRules.Add(application);
            }

            if (_Enabled)
            {
                UpdateAll();
            }
        }

        public void RemoveAlwaysAllowed(ApplicationInfo application)
        {
            _AlwaysAllowed.ApplicationRules.Remove(application.Id);

            if (_Enabled)
            {
                UpdateAll();
            }
        }

        public void AddAlwaysBlocked(IEnumerable<ApplicationInfo> applications)
        {
            foreach (ApplicationInfo application in applications)
            {
                _AlwaysAllowed.ApplicationRules.Remove(application.Id);
                _AlwaysBlocked.ApplicationRules.Add(application);
            }

            if (_Enabled)
            {
                UpdateAll();
            }
        }

        public void RemoveAlwaysBlocked(ApplicationInfo application)
        {
            _AlwaysBlocked.ApplicationRules.Remove(application.Id);

            if (_Enabled)
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
            else if (_AlwaysAllowed.ApplicationRules.Contains(executablePath))
            {
                return false;
            }
            else if (_AlwaysBlocked.ApplicationRules.Contains(executablePath))
            {
                return true;
            }
            else if (_ActiveEvent == null)
            {
                return false;
            }
            else if (_ActiveEvent.Type == EventType.OfflineBreak)
            {
                return true;
            }
            else if (_ActiveEvent.Rules != null && _ActiveEvent.Rules.ApplicationRules.IsBlocked(executablePath))
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
            if (!_BlockedApplicationOverlays.ContainsKey(handle))
            {
                _BlockedApplicationOverlays.Add(handle, new BlockedOverlayView(handle));
            }
        }

        private void TryUnblockApplication(IntPtr handle)
        {
            if (_BlockedApplicationOverlays.Remove(handle, out BlockedOverlayView? overlay))
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
