using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zarp.Core
{

    public class Event
    {
        public string Name;
        public int Duration;
        public TimeUnit DurationUnit;
        public EventType Type;
        public RulePreset? Rules;

        public Event(string name, int duration, TimeUnit unit, EventType type, RulePreset? rules)
        {
            Name = name;
            Duration = duration;
            DurationUnit = unit;
            Type = type;
            Rules = rules;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public enum EventType
    {
        Regular, OfflineBreak
    }
}
