namespace Zarp.Core.Datatypes
{
    internal class FocusSessionEvent
    {
        public string Name;
        public int Duration;
        public EventType Type;
        public RuleSet? Rules;

        public FocusSessionEvent(string name, int duration, EventType type, RuleSet? rules)
        {
            Name = name;
            Duration = duration;
            Type = type;
            Rules = rules;
        }

        public FocusSessionEvent(FocusSessionEvent other)
        {
            Name = other.Name;
            Duration = other.Duration;
            Type = other.Type;
            Rules = other.Rules;
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
