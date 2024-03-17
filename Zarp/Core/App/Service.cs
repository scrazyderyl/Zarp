using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using Zarp.Core.Datatypes;
using Zarp.GUI.View;
using static Zarp.Common.Util.PInvoke;

namespace Zarp.Core.App
{
    internal class Service
    {
        internal static object? DialogReturnValue;

        internal static string UserDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Zarp\";

        internal static PresetCollection<FocusSession> FocusSessionPresets = new PresetCollection<FocusSession>();
        internal static PresetCollection<RuleSet> RulePresets = new PresetCollection<RuleSet>();
        internal static PresetCollection<Reward> RewardPresets = new PresetCollection<Reward>();

        internal static RuleSet _AlwaysAllowed;
        internal static RuleSet _AlwaysBlocked;

        private static Dictionary<IntPtr, BlockedOverlayView> _BlockedApplicationOverlays;
        private static Dictionary<Guid, Reward> _EnabledRewards;
        private static FocusSession? _ActiveFocusSession;
        private static FocusSessionEvent? _ActiveFocusSessionEvent;
        private static bool _Enabled;

        private static WinEventDelegate _ForegroundEventHandler;
        private static IntPtr _ForegroundEventHook;

        static Service()
        {
            _AlwaysAllowed = new RuleSet();
            _AlwaysBlocked = new RuleSet();

            _BlockedApplicationOverlays = new Dictionary<IntPtr, BlockedOverlayView>();
            _EnabledRewards = new Dictionary<Guid, Reward>();
            _ActiveFocusSessionEvent = null;
            _Enabled = false;

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            _ForegroundEventHandler = OnForegroundChanged;
            Enable();
        }

        public static FocusSession? ActiveFocusSession
        {
            get => _ActiveFocusSession;
        }

        public static FocusSessionEvent? ActiveEvent
        {
            get => _ActiveFocusSessionEvent;
        }

        private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {

        }

        public static void Enable()
        {
            _Enabled = true;
            _ForegroundEventHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _ForegroundEventHandler, 0, 0, WINEVENT_OUTOFCONTEXT);
            UpdateAll();
        }

        public static void Disable()
        {
            _Enabled = false;

            foreach (BlockedOverlayView window in _BlockedApplicationOverlays.Values)
            {
                window.Close();
            }

            _BlockedApplicationOverlays.Clear();
            UnhookWinEvent(_ForegroundEventHook);
        }

        private static void OnForegroundChanged(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint isEventThread, uint dwmsEventTime)
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

        internal static void WindowClosed(IntPtr handle)
        {
            _BlockedApplicationOverlays.Remove(handle);
        }

        public static void SetActiveEvent(FocusSessionEvent newEvent)
        {
            _ActiveFocusSessionEvent = newEvent;
            UpdateAll();
        }

        public static void ClearActiveEvent()
        {
            _ActiveFocusSessionEvent = null;
            UpdateAll();
        }

        public static void EnableReward(Reward reward)
        {
            _EnabledRewards.TryAdd(reward.Guid, reward);
        }

        public static void DisableReward(Reward reward)
        {
            _EnabledRewards.Remove(reward.Guid);
        }

        public static bool IsRewardEnabled(Reward reward)
        {
            return _EnabledRewards.ContainsKey(reward.Guid);
        }

        public static void AddAlwaysAllowed(IEnumerable<ApplicationInfo> applications)
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

        public static void RemoveAlwaysAllowed(ApplicationInfo application)
        {
            _AlwaysAllowed.ApplicationRules.Remove(application.Id);

            if (_Enabled)
            {
                UpdateAll();
            }
        }

        public static void AddAlwaysBlocked(IEnumerable<ApplicationInfo> applications)
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

        public static void RemoveAlwaysBlocked(ApplicationInfo application)
        {
            _AlwaysBlocked.ApplicationRules.Remove(application.Id);

            if (_Enabled)
            {
                UpdateAll();
            }
        }

        public static bool IsApplicationBlocked(IntPtr handle)
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
            else if (_ActiveFocusSessionEvent == null)
            {
                return false;
            }
            else if (_ActiveFocusSessionEvent.Type == EventType.OfflineBreak)
            {
                return true;
            }
            else if (_ActiveFocusSessionEvent.Rules != null && _ActiveFocusSessionEvent.Rules.ApplicationRules.IsBlocked(executablePath))
            {
                return true;
            }

            return false;
        }

        public static void UpdateAll()
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

        private static void TryBlockApplication(IntPtr handle)
        {
            if (!_BlockedApplicationOverlays.ContainsKey(handle))
            {
                _BlockedApplicationOverlays.Add(handle, new BlockedOverlayView(handle));
            }
        }

        private static void TryUnblockApplication(IntPtr handle)
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
