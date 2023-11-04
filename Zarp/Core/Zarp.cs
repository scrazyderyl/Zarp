using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Zarp.Core.PInvoke;

namespace Zarp.Core
{
    class Zarp
    {
        EventManager eventManager;

        private static string[] AlwaysIgnored = new string[] { "", "Zarp", "Settings", "Program Manager", "Microsoft Text Input Application" };
        private HashSet<string> IgnoredApps = new HashSet<string>(AlwaysIgnored);

        public Zarp()
        {
            eventManager = new EventManager();

            SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, new WinEventDelegate(OnForegroundWindowChanged), 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        public void OnForegroundWindowChanged(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint isEventThread, uint dwmsEventTime)
        {

        }
    }
}
