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
        public DurationUnit Unit;
        public EventType Type;
        public RulePreset? Rules;

        public Event()
        {
            Name = String.Empty;
            Duration = 30;
            Unit = DurationUnit.Minutes;
            Type = EventType.Regular;
            Rules = null;
        }

        public Event(string name, int duration, DurationUnit unit, EventType type, RulePreset rules)
        {
            Name = name;
            Duration = duration;
            Unit = unit;
            Type = type;
            Rules = rules;
        }

        public bool IsApplicationBlocked(string executablePath)
        {
            if (Type == EventType.OfflineBreak)
            {
                return true;
            }

            return Rules.IsApplicationBlocked(executablePath);
        }

        public bool IsWebsiteBlocked(string domain)
        {
            if (Type == EventType.OfflineBreak)
            {
                return true;
            }

            return Rules.IsWebsiteBlocked(domain);
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

    public enum DurationUnit
    {
        Minutes, Hours
    }
}
