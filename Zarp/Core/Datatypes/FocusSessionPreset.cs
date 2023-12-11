using System.Collections.Generic;
using System.Linq;

namespace Zarp.Core.Datatypes
{
    public class FocusSessionPreset : Preset
    {
        public int LoopCount { get; set; }

        private List<Event> Events;
        private int Duration;

        public FocusSessionPreset(string name, int loopCount)
        {
            Name = name;
            LoopCount = loopCount;

            Events = new List<Event>();
            Duration = 0;
        }

        public void NewEvent(Event _event)
        {
            Events.Add(_event);
            Duration += _event.Duration;
        }

        public void AddEventAtIndex(int index, Event _event)
        {
            Events.Insert(index, _event);
            Duration += _event.Duration;
        }

        public void SwapEvents(int index1, int index2)
        {
            Event temp = Events[index1];
            Events[index1] = Events[index2];
            Events[index2] = temp;
        }

        public void RemoveEvent(int index)
        {
            Events.RemoveAt(index);
        }

        public Event? GetEventByTime(int time)
        {
            if (time / Duration > LoopCount)
            {
                return null;
            }

            time = time % Duration;
            int sumDuration = 0;

            foreach (Event _event in Events)
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
            return Events[index];
        }

        public IEnumerable<Event> GetEvents()
        {
            return Events;
        }
    }
}
