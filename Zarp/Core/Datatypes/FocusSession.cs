using System;
using System.Collections.Generic;
using System.Linq;
using Zarp.Core.App;

namespace Zarp.Core.Datatypes
{
    internal class FocusSession : Preset
    {
        public int LoopCount;

        internal List<FocusSessionEvent> _Events;
        internal Dictionary<Guid, ScheduleEvent> _Dependents;

        public int Duration
        {
            get
            {
                int total = 0;

                foreach (FocusSessionEvent item in _Events)
                {
                    total += item.Duration;
                }

                return total * LoopCount;
            }
        }

        public FocusSession(string name, int loopCount) : base(name)
        {
            _Name = name;
            LoopCount = loopCount;
            _Events = new List<FocusSessionEvent>();
            _Dependents = new Dictionary<Guid, ScheduleEvent>();
        }

        public FocusSession(string name, FocusSession other) : base(name)
        {
            _Name = name;
            LoopCount = other.LoopCount;
            _Events = new List<FocusSessionEvent>(other._Events.Count);

            foreach (FocusSessionEvent item in other._Events)
            {
                _Events.Add(new FocusSessionEvent(item));
                item.Rules?._Dependents.TryAdd(Guid, this);
            }

            _Dependents = new Dictionary<Guid, ScheduleEvent>();
        }

        public void SetEventRuleSet(FocusSessionEvent focusSessionEvent, RuleSet? newRuleSet)
        {
            if (focusSessionEvent.Rules == newRuleSet)
            {
                return;
            }

            RuleSet? oldRuleSet = focusSessionEvent.Rules;
            newRuleSet?._Dependents.TryAdd(Guid, this);
            focusSessionEvent.Rules = newRuleSet;

            if (oldRuleSet != null)
            {
                UpdateDependency(oldRuleSet);
            }
        }

        private void UpdateDependency(RuleSet ruleSet)
        {
            foreach (FocusSessionEvent focusSessionEvent in _Events)
            {
                if (focusSessionEvent.Rules == ruleSet)
                {
                    return;
                }
            }

            ruleSet._Dependents.Remove(Guid);
        }

        public FocusSessionEvent? GetEventByTime(int time)
        {
            if (time / Duration > LoopCount)
            {
                return null;
            }

            time %= Duration;
            int cumulativeDuration = 0;

            foreach (FocusSessionEvent item in _Events)
            {
                cumulativeDuration += item.Duration;

                if (time < cumulativeDuration)
                {
                    return item;
                }
            }

            return null;
        }

        public override bool IsModifiable => Service.ActiveFocusSession != this;

        public override bool IsDeletable => Service.ActiveFocusSession != this && _Dependents.Count == 0;

        public override Preset Duplicate(string name) => new FocusSession(name, this);

        public override string Type => "Focus Sesssion";

        public override IEnumerable<IDependency>? Dependencies
        {
            get
            {
                HashSet<Guid> uniqueRulesets = new HashSet<Guid>(_Events.Count);

                foreach (FocusSessionEvent focusSessionEvent in _Events)
                {
                    if (focusSessionEvent.Rules != null && uniqueRulesets.Add(focusSessionEvent.Rules.Guid))
                    {
                        yield return focusSessionEvent.Rules;
                    }
                }
            }
        }

        public override IEnumerable<IDependency>? Dependents => _Dependents.Cast<IDependency>();
    }
}
