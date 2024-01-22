using System.Collections.Generic;

namespace Zarp.Core.Datatypes
{
    public class FocusSessionPreset : Preset
    {
        public int LoopCount { get; set; }
        public IEnumerable<Event> Events => _Events;
        public List<Event> _Events;
        public int Duration
        {
            get => _Duration;
        }

        private int _Duration;

        public FocusSessionPreset(string name, int loopCount) : base(name)
        {
            LoopCount = loopCount;

            _Events = new List<Event>();
            _Duration = 0;
        }

        public FocusSessionPreset(string name, FocusSessionPreset preset) : base(name)
        {
            LoopCount = preset.LoopCount;

            _Events = new List<Event>(preset._Events.Count);

            foreach (Event e in preset._Events)
            {
                _Events.Add(new Event(e));
            }

            _Duration = preset._Duration;
        }

        public void NewEvent(Event _event)
        {
            _Events.Add(_event);
            _Duration += _event.Duration;
        }

        public void AddEventAtIndex(int index, Event _event)
        {
            _Events.Insert(index, _event);
            _Duration += _event.Duration;
        }

        public void SwapEvents(int index1, int index2)
        {
            Event temp = _Events[index1];
            _Events[index1] = _Events[index2];
            _Events[index2] = temp;
        }

        public void RemoveEvent(int index)
        {
            _Events.RemoveAt(index);
        }

        public Event? GetEventByTime(int time)
        {
            if (time / _Duration > LoopCount)
            {
                return null;
            }

            time = time % _Duration;
            int sumDuration = 0;

            foreach (Event _event in _Events)
            {
                if (_event.DurationUnit == TimeUnit.Minutes)
                {
                    sumDuration += _event.Duration;
                }
                else
                {
                    sumDuration += _event.Duration * 60;
                }

                if (time < sumDuration)
                {
                    return _event;
                }
            }

            return null;
        }

        public Event? GetEventByIndex(int index)
        {
            return _Events[index];
        }

        public override Preset Duplicate(string name) => new FocusSessionPreset(name, this);
    }
}
