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

        string route = "C:\\Users\\webju\\OneDrive - Žilinská univerzita v Žiline\\Bakalarska praca\\data\\Log_Simulate.txt";

        private ImmutableList<Bus> _buses;
        // kalendar udalosti kde prirota je cas a element je udalost
        private readonly PriorityQueue<Event, int> _eventCalendar;
        private Individual _individual;
        

        public DiscreteEventSimulation(Individual individual)
        {
            this._buses = DataReader.GetInstance().GetBuses();
            this._eventCalendar = new PriorityQueue<Event, int>();
            this._individual = individual;
        }

        public void simulate()
        {
            using (var writer = File.CreateText(route))
            {

                foreach (Base.DayOfWeek day in Enum.GetValues(typeof(Base.DayOfWeek)))
                {
                    InitSimulate(day);

                    while (_eventCalendar.Count > 0)
                    {
                        Event currentEvent = this._eventCalendar.Dequeue();
                        currentEvent.Trigger();

                        writer.Write(currentEvent.ToString());

                        if (currentEvent is ArriveEvent)
                        {
                            this.PlanNextEvent((ArriveEvent)currentEvent);
                        }
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
                    var relocationEvent = new RelocationEvent(bus, relocation.BusStop, relocation.Time ,lineSchedule.GetFirstBusStopSchedule().BusStop);
                    this._eventCalendar.Enqueue(relocationEvent, relocation.Time);
                }

                var arriveEvent = new ArriveEvent(bus, lineSchedule.GetLastBusStopSchedule().BusStop, lineSchedule.GetEndTime(), lineSchedule);
                this._eventCalendar.Enqueue(arriveEvent, lineSchedule.GetEndTime());
            }
        }

        // Ked autobus ukonci 
        private void PlanNextEvent(ArriveEvent currentEvent)
        {
            var bus = currentEvent.Bus;
            var lineSchedule = currentEvent.LineSchedule;

            var nextLineSchedule = bus.GetNextLineSchedule(lineSchedule);

            var actualBusStop = lineSchedule.GetLastBusStopSchedule().BusStop;
            
            // Koniec harmonagramu pre autobus presun do depa
            if (nextLineSchedule == null)
            {
                if (actualBusStop.Equals(bus.GetEndDepo().BusStop))
                {
                    return;
                }

                var relocationEvent = new RelocationEvent(bus, actualBusStop, lineSchedule.GetEndTime() + 1,  bus.GetEndDepo().BusStop);
                this._eventCalendar.Enqueue(relocationEvent, lineSchedule.GetEndTime() + 1);
                return;
            }
            
            var nextBusStop = nextLineSchedule.GetFirstBusStopSchedule().BusStop;

            // riesenie presunu autobusu
            if (actualBusStop.Equals(nextBusStop))
            {
                if (this._individual.FreeChargingPoint(actualBusStop))
                {
                    var chargingEvent = new ChargingEvent(bus, actualBusStop, nextLineSchedule.GetStartTime() - 1, lineSchedule.GetEndTime(), nextLineSchedule.GetStartTime() - 1);
                    this._eventCalendar.Enqueue(chargingEvent, nextLineSchedule.GetStartTime()-1);

                    this._individual.UseChargingPoint(actualBusStop);
                }
            }
            else
            {
                var actualChargingPoint = this._individual.IsChargingPointFree(actualBusStop);
                var nextChargingPoint = this._individual.IsChargingPointFree(nextBusStop);

                if (actualChargingPoint || nextChargingPoint)
                {

                    if (!actualChargingPoint)
                    {
                        var relocationEvent = new RelocationEvent(bus, actualBusStop, lineSchedule.GetEndTime() + 1, nextBusStop);
                        this._eventCalendar.Enqueue(relocationEvent, lineSchedule.GetEndTime() + 1);
                        var chargingEvent = new ChargingEvent(bus, nextBusStop, nextLineSchedule.GetStartTime() - 1, lineSchedule.GetEndTime() + 2, nextLineSchedule.GetStartTime() - 1);
                        this._eventCalendar.Enqueue(chargingEvent, nextLineSchedule.GetStartTime() - 1);

                        this._individual.UseChargingPoint(nextBusStop);
                    }
                    else // if (nextChargingPoint == 0)
                    {
                        var chargingEvent = new ChargingEvent(bus, actualBusStop, nextLineSchedule.GetStartTime() - 2, lineSchedule.GetEndTime(), nextLineSchedule.GetStartTime() - 2);
                        this._eventCalendar.Enqueue(chargingEvent, nextLineSchedule.GetStartTime() - 2);
                        var relocationEvent = new RelocationEvent(bus, actualBusStop, nextLineSchedule.GetStartTime() - 1, nextBusStop);
                        this._eventCalendar.Enqueue(relocationEvent, nextLineSchedule.GetStartTime() - 1);

                        this._individual.UseChargingPoint(actualBusStop);
                    }
                    //else
                    //{
                    //    // TODO 
                    //    // vyriesit ako sa bude spravat ak su nabijacie body na oboch zastavkach
                    //}
                }
                else 
                {
                    var relocationEvent = new RelocationEvent(bus, actualBusStop, nextLineSchedule.GetStartTime() - 3, nextBusStop);
                    this._eventCalendar.Enqueue(relocationEvent, nextLineSchedule.GetStartTime() - 3);
                }

            }

            var arriveEvent = new ArriveEvent(bus, nextLineSchedule.GetLastBusStopSchedule().BusStop, nextLineSchedule.GetEndTime(), nextLineSchedule);
            this._eventCalendar.Enqueue(arriveEvent, nextLineSchedule.GetEndTime());
        }

    }
}
