using System.Collections.Generic;
using System.Linq;

namespace Zarp.Core
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

        private void AddEventAtIndex(int index, Event _event)
        {
            Events.Insert(index, _event);
            Duration += _event.Duration;
        }

        private void SwapEvents(int index1, int index2)
        {
            Event temp = Events[index1];
            Events[index1] = Events[index2];
            Events[index2] = temp;
        }

        private void RemoveEvent(int index)
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
                sumDuration += _event.Duration;

                if (time <  sumDuration)
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
