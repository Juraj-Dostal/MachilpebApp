using MachilpebLibrary.Algorithm;
using MachilpebLibrary.Base;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachilpebLibrary.Simulation
{
    public class DiscreteEventSimulation
    {

        private ImmutableList<Bus> _buses;
        // kalendar udalosti kde prirota je cas a element je udalost
        private PriorityQueue<Event, int> _eventCalendar;
        private Individual _individual;
        

        public DiscreteEventSimulation()
        {
            _buses = DataReader.GetInstance().GetBuses();
            _eventCalendar = new PriorityQueue<Event, int>();
            _individual = new Individual();
        }

        public void simulate()
        {
            while (_eventCalendar.Count > 0)
            {
                var currentEvent = _eventCalendar.Dequeue();
                var bus = currentEvent.Bus;
                var eventType = currentEvent.EventType;

                switch (eventType)
                {
                    case EventType.Start:
                        // pridanie udalosti na koniec trasy
                        _eventCalendar.Enqueue(new Event(bus, EventType.End), bus.EndTime);
                        break;
                    case EventType.End:
                        // pridanie udalosti na zaciatok trasy
                        _eventCalendar.Enqueue(new Event(bus, EventType.Start), bus.StartTime);
                        break;
                    default:
                        throw new ArgumentException("Unknown event type");
                }
            }
        }

        private void InitSimulate(Base.DayOfWeek day)
        {
            var buses = _buses.Where(bus => bus.Day == day).ToList();

            foreach (var bus in buses)
            {
                _eventCalendar.Enqueue(new Event(bus, EventType.Start), bus.StartTime);
            }

        }





    }
}
