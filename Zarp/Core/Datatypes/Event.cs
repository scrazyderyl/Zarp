namespace Zarp.Core.Datatypes
{

    internal class Event
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

        public Event(Event e)
        {
            Name = e.Name;
            Duration = e.Duration;
            DurationUnit = e.DurationUnit;
            Type = e.Type;
            Rules = e.Rules;
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
