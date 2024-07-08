using System;
using System.Collections.Generic;
using static Zarp.Common.PInvoke.User32;

namespace Zarp.Core.App
{
    internal class ProcessWinEventListener : IDisposable
    {
        private const uint EVENT_MIN = 0x00000001;
        private const uint EVENT_MAX = 0x7FFFFFFF;

        public delegate void WinEventHandler(IntPtr hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime);

        private uint _ProcessId;
        private Dictionary<uint, WinEventHandler> _Events;
        private WinEventDelegate _EventHandler;
        private IntPtr _EventHook;

        public ProcessWinEventListener(int processId)
        {
            _ProcessId = (uint)processId;
            _Events = new Dictionary<uint, WinEventHandler>();
            _EventHandler = OnWinEvent;
        }

        private void OnWinEvent(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
        {
            if (_Events.TryGetValue(eventType, out WinEventHandler? existing))
            {
                existing.Invoke(hwnd, idObject, idChild, idEventThread, dwmsEventTime);
            }
        }

        public bool Enable()
        {
            _EventHook = SetWinEventHook(EVENT_MIN, EVENT_MAX, IntPtr.Zero, _EventHandler, _ProcessId, 0, WINEVENT_OUTOFCONTEXT);

            return _EventHook == IntPtr.Zero;
        }

        public bool Disable()
        {
            return UnhookWinEvent(_EventHook);
        }

        public void Subscribe(uint eventConstant, WinEventHandler handler)
        {
            if (_Events.TryGetValue(eventConstant, out WinEventHandler? existing))
            {
                _Events[eventConstant] += handler;
            }
            else
            {
                _Events.Add(eventConstant, handler);
            }
        }

        public void Unsubscribe(uint eventConstant, WinEventHandler handler)
        {
            if (_Events.TryGetValue(eventConstant, out WinEventHandler? existing))
            {
                WinEventHandler? result = existing - handler;

                if (result == null)
                {
                    _Events.Remove(eventConstant);
                }
                else
                {
                    _Events[eventConstant] = result;
                }
            }
        }

        public void Dispose()
        {
            UnhookWinEvent(_EventHook);
        }
    }
}
