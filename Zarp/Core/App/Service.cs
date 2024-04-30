using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using Zarp.Core.Datatypes;
using Zarp.GUI.View;
using static Zarp.Common.Util.PInvoke;
using static Zarp.Common.Util.Window;

namespace Zarp.Core.App
{
    internal class Service
    {
        internal static object? DialogReturnValue;

        internal static string UserDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Zarp\";

        internal static PresetCollection<FocusSession> FocusSessions = new PresetCollection<FocusSession>();
        internal static PresetCollection<RuleSet> RuleSets = new PresetCollection<RuleSet>();
        internal static PresetCollection<Reward> Rewards = new PresetCollection<Reward>();

        internal static RuleSet AlwaysAllowed;
        internal static RuleSet AlwaysBlocked;

        private static Dictionary<IntPtr, BlockedOverlayView> BlockedApplicationOverlays;
        private static Dictionary<Guid, Reward> EnabledRewards;
        internal static FocusSession? ActiveFocusSession;
        private static FocusSessionEvent? ActiveFocusSessionEvent;
        private static bool Enabled;

        private static WinEventDelegate ForegroundEventHandler;
        private static IntPtr ForegroundEventHook;

        public static FocusSessionEvent? ActiveEvent
        {
            get => ActiveFocusSessionEvent;
        }

        static Service()
        {
            AlwaysAllowed = new RuleSet();
            AlwaysBlocked = new RuleSet();

            BlockedApplicationOverlays = new Dictionary<IntPtr, BlockedOverlayView>();
            EnabledRewards = new Dictionary<Guid, Reward>();
            ActiveFocusSessionEvent = null;
            Enabled = false;

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            ForegroundEventHandler = OnForegroundChanged;
            Enable();
        }

        private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {

        }

        public static void Enable()
        {
            Enabled = true;
            ForegroundEventHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, ForegroundEventHandler, 0, 0, WINEVENT_OUTOFCONTEXT);
            UpdateAll();
        }

        public static void Disable()
        {
            Enabled = false;

            foreach (BlockedOverlayView window in BlockedApplicationOverlays.Values)
            {
                window.Close();
            }

            BlockedApplicationOverlays.Clear();
            UnhookWinEvent(ForegroundEventHook);
        }

        private static void OnForegroundChanged(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint isEventThread, uint dwmsEventTime)
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

        internal static void WindowClosed(IntPtr handle)
        {
            BlockedApplicationOverlays.Remove(handle);
        }

        public static void SetActiveEvent(FocusSessionEvent newEvent)
        {
            ActiveFocusSessionEvent = newEvent;
            UpdateAll();
        }

        public static void ClearActiveEvent()
        {
            ActiveFocusSessionEvent = null;
            UpdateAll();
        }

        public static void EnableReward(Reward reward)
        {
            EnabledRewards.TryAdd(reward.Guid, reward);
        }

        public static void DisableReward(Reward reward)
        {
            EnabledRewards.Remove(reward.Guid);
        }

        public static bool IsRewardEnabled(Reward reward)
        {
            return EnabledRewards.ContainsKey(reward.Guid);
        }

        public static void AddAlwaysAllowed(IEnumerable<ApplicationInfo> applications)
        {
            foreach (ApplicationInfo application in applications)
            {
                AlwaysBlocked._ApplicationRules.Remove(application);
                AlwaysAllowed._ApplicationRules.Add(application);
            }

            if (Enabled)
            {
                UpdateAll();
            }
        }

        public static void RemoveAlwaysAllowed(ApplicationInfo application)
        {
            AlwaysAllowed._ApplicationRules.Remove(application);

            if (Enabled)
            {
                UpdateAll();
            }
        }

        public static void AddAlwaysBlocked(IEnumerable<ApplicationInfo> applications)
        {
            foreach (ApplicationInfo application in applications)
            {
                AlwaysAllowed._ApplicationRules.Remove(application);
                AlwaysBlocked._ApplicationRules.Add(application);
            }

            if (Enabled)
            {
                UpdateAll();
            }
        }

        public static void RemoveAlwaysBlocked(ApplicationInfo application)
        {
            AlwaysBlocked._ApplicationRules.Remove(application);

            if (Enabled)
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

            ApplicationInfo info = new ApplicationInfo(executablePath);

            if (AlwaysAllowed._ApplicationRules.Contains(info))
            {
                return false;
            }
            else if (AlwaysBlocked._ApplicationRules.Contains(info))
            {
                return true;
            }
            else if (ActiveFocusSessionEvent == null)
            {
                return false;
            }
            else if (ActiveFocusSessionEvent.Type == EventType.OfflineBreak)
            {
                return true;
            }
            else if (ActiveFocusSessionEvent.Rules != null && ActiveFocusSessionEvent.Rules.IsApplicationBlocked(info))
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
            if (!BlockedApplicationOverlays.ContainsKey(handle))
            {
                BlockedApplicationOverlays.Add(handle, new BlockedOverlayView(handle));
            }
        }

        private static void TryUnblockApplication(IntPtr handle)
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
