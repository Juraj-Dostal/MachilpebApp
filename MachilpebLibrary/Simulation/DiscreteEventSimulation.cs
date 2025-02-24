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
            this._buses = DataReader.GetInstance().GetBuses();
            this._eventCalendar = new PriorityQueue<Event, int>();
            this._individual = new Individual();
        }

        public void simulate()
        {
            foreach (Base.DayOfWeek day in Enum.GetValues(typeof(Base.DayOfWeek)))
            {
                InitSimulate(day);

                while (_eventCalendar.Count > 0)
                {
                    Event currentEvent = this._eventCalendar.Dequeue();
                    currentEvent.Trigger();

                    if (currentEvent is ArriveEvent)
                    {
                        this.PlanNextEvent((ArriveEvent)currentEvent);
                    }
                }
            }
        }

        private void InitSimulate(Base.DayOfWeek day)
        {
            var buses = _buses.Where(bus => bus.Day == day).ToList();

            foreach (var bus in buses)
            {
                bus.ChargeBattery();

                var relocation = bus.GetDistanceFromDepo();
                var lineSchedule = bus.GetNextLineSchedule();

                if (lineSchedule == null) 
                {
                    throw new Exception("Line schedule not found");
                }

                if (relocation != null)
                { 
                    var relocationEvent = new RelocationEvent(bus, relocation.BusStop, lineSchedule.GetFirstBusStopSchedule().BusStop);
                    this._eventCalendar.Enqueue(relocationEvent, relocation.Time);
                }

                var arriveEvent = new ArriveEvent(bus, lineSchedule.GetLastBusStopSchedule().BusStop, lineSchedule);

                this._eventCalendar.Enqueue(arriveEvent, lineSchedule.GetEndTime());
            }
        }

        // Ked autobus ukonci 
        private void PlanNextEvent(ArriveEvent currentEvent)
        {
            var bus = currentEvent.Bus;
            var lineSchedule = currentEvent.LineSchedule;

            var nextLineSchedule = bus.GetNextLineSchedule(lineSchedule);

            if (nextLineSchedule == null)
            {
                return;
            }

            var actualBusStop = lineSchedule.GetLastBusStopSchedule().BusStop;
            var nextBusStop = nextLineSchedule.GetFirstBusStopSchedule().BusStop;

            // riesenie presunu autobusu
            if (actualBusStop.Equals(nextBusStop))
            {
                if (this._individual.GetChargingPoint(actualBusStop) > 0)
                {
                    var chargingEvent = new ChargingEvent(bus, actualBusStop, lineSchedule.GetEndTime(), nextLineSchedule.GetStartTime() - 1);
                    this._eventCalendar.Enqueue(chargingEvent, nextLineSchedule.GetStartTime()-1);
                }
            }
            else
            {
                var actualChargingPoint = this._individual.GetChargingPoint(actualBusStop);
                var nextChargingPoint = this._individual.GetChargingPoint(nextBusStop);

                if (actualChargingPoint > 0 || nextChargingPoint > 0)
                {

                    if (actualChargingPoint == 0)
                    {
                        var relocationEvent = new RelocationEvent(bus, actualBusStop, nextBusStop);
                        this._eventCalendar.Enqueue(relocationEvent, lineSchedule.GetEndTime() + 1);
                        var chargingEvent = new ChargingEvent(bus, nextBusStop, lineSchedule.GetEndTime() + 2, nextLineSchedule.GetStartTime() - 1);
                        this._eventCalendar.Enqueue(chargingEvent, nextLineSchedule.GetStartTime() - 1);
                    }
                    else // if (nextChargingPoint == 0)
                    {
                        var chargingEvent = new ChargingEvent(bus, actualBusStop, lineSchedule.GetEndTime(), nextLineSchedule.GetStartTime() - 2);
                        this._eventCalendar.Enqueue(chargingEvent, nextLineSchedule.GetStartTime() - 2);
                        var relocationEvent = new RelocationEvent(bus, actualBusStop, nextBusStop);
                        this._eventCalendar.Enqueue(relocationEvent, nextLineSchedule.GetStartTime() - 1);
                    }
                    //else
                    //{
                    //    // TODO 
                    //    // vyriesit ako sa bude spravat ak su nabijacie body na oboch zastavkach
                    //}
                }
                else 
                {
                    var relocationEvent = new RelocationEvent(bus, actualBusStop, nextBusStop);
                    this._eventCalendar.Enqueue(relocationEvent, nextLineSchedule.GetStartTime() - 3);
                }

            }


            var arriveEvent = new ArriveEvent(bus, nextLineSchedule.GetLastBusStopSchedule().BusStop, nextLineSchedule);
            this._eventCalendar.Enqueue(arriveEvent, nextLineSchedule.GetEndTime());
            
        }




    }
}
