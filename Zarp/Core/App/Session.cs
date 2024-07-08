using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using Zarp.Core.Datatypes;
using Zarp.GUI.View;
using static Zarp.Common.PInvoke.User32;
using static Zarp.Common.Util.Window;

namespace Zarp.Core.App
{
    internal class Session
    {
        internal static object? DialogReturnValue;

        internal static RuleSet AlwaysAllowedApplications;
        internal static RuleSet AlwaysBlockedApplications;
        internal static FocusSession? ActiveFocusSession;
        internal static Dictionary<Guid, Reward> EnabledRewards;
        internal static FocusSessionEvent? ActiveFocusSessionEvent;
        private static Dictionary<IntPtr, BlockedOverlayView> BlockedApplicationOverlays;
        private static bool Enabled;

        static Session()
        {
            AlwaysAllowedApplications = new RuleSet();
            AlwaysBlockedApplications = new RuleSet();
            EnabledRewards = new Dictionary<Guid, Reward>();
            ActiveFocusSessionEvent = null;

            BlockedApplicationOverlays = new Dictionary<IntPtr, BlockedOverlayView>();
            Enabled = false;

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            Enable();
        }

        private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {

        }

        public static void Enable()
        {
            Enabled = true;
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
                AlwaysBlockedApplications._ApplicationRules.Remove(application);
                AlwaysAllowedApplications._ApplicationRules.Add(application);
            }

            if (Enabled)
            {
                UpdateAll();
            }
        }

        public static void RemoveAlwaysAllowed(ApplicationInfo application)
        {
            AlwaysAllowedApplications._ApplicationRules.Remove(application);

            if (Enabled)
            {
                UpdateAll();
            }
        }

        public static void AddAlwaysBlocked(IEnumerable<ApplicationInfo> applications)
        {
            foreach (ApplicationInfo application in applications)
            {
                AlwaysAllowedApplications._ApplicationRules.Remove(application);
                AlwaysBlockedApplications._ApplicationRules.Add(application);
            }

            if (Enabled)
            {
                UpdateAll();
            }
        }

        public static void RemoveAlwaysBlocked(ApplicationInfo application)
        {
            AlwaysBlockedApplications._ApplicationRules.Remove(application);

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

            try
            {
                ApplicationInfo info = new ApplicationInfo(executablePath);
                return IsApplicationBlocked(info);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsApplicationBlocked(ApplicationInfo info)
        {
            if (AlwaysAllowedApplications._ApplicationRules.Contains(info))
            {
                return false;
            }
            else if (AlwaysBlockedApplications._ApplicationRules.Contains(info))
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
