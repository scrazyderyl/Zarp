using System.Collections.Generic;

namespace Zarp.Core.Datatypes
{
    internal class FocusSessionPreset : IPreset
    {
        public string Name { get; set; }
        public int LoopCount;
        public int Duration
        {
            get
            {
                int total = 0;

                foreach (Event item in Events)
                {
                    total += item.Duration;
                }

                return total;
            }
        }

        internal List<Event> Events;

        public FocusSessionPreset()
        {
            Name = string.Empty;
            Events = new List<Event>();
        }

        public FocusSessionPreset(string name, int loopCount)
        {
            Name = name;
            LoopCount = loopCount;
            Events = new List<Event>();
        }

        public FocusSessionPreset(string name, FocusSessionPreset preset)
        {
            Name = name;
            LoopCount = preset.LoopCount;
            Events = new List<Event>(preset.Events.Count);

            foreach (Event item in preset.Events)
            {
                Events.Add(new Event(item));
            }
        }

        public Event? GetEventByTime(int time)
        {
            int duration = Duration;

            if (time / duration > LoopCount)
            {
                return null;
            }

            time %= duration;
            int cumulativeDuration = 0;

            foreach (Event item in Events)
            {
                switch (item.DurationUnit)
                {
                    case TimeUnit.Minutes:
                        cumulativeDuration += item.Duration;
                        break;
                    case TimeUnit.Hours:
                        cumulativeDuration += item.Duration * 60;
                        break;
                }

                if (time < cumulativeDuration)
                {
                    return item;
                }
            }

            return null;
        }

        public IPreset Duplicate(string name) => new FocusSessionPreset(name, this);
    }
}
