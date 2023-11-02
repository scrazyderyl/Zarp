using System;

namespace Zarp.Core
{
    class Scheduler
    {
        public void Save()
        {

        }
    }

    interface Frequency
    {
        bool isActive(DateTime time);
    }

    struct Once : Frequency
    {
        public int StartTime;
        public int Duration;

        public Once(int startTime, int duration)
        {
            StartTime = startTime;
            Duration = duration;
        }

        public bool isActive(DateTime time)
        {
            return true;
        }
    }

    struct FocusSession
    {
        public FocusSessionPreset Preset;
        public DateTime Start;

        public FocusSession(FocusSessionPreset preset, DateTime start)
        {
            Preset = preset;
            Start = start;
        }
    }
}
