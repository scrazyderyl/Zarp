using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zarp.Core
{
    public class Event
    {
        public int Duration;
        public EventType Type;
        public RulePreset Rules;

        public Event(int duration, EventType type, RulePreset rules)
        {
            Duration = duration;
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
            return Rules.Name;
        }
    }
}
