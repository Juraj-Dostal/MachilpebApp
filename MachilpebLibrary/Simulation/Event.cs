using MachilpebLibrary.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachilpebLibrary.Simulation
{

    /*
     * Trieda reprezentujuca udalost v simulacii
     * 
     * 
     * 
     */
    public abstract class Event
    {

        public Bus Bus { get; }
        public BusStop BusStop { get; }

        public Event(Bus bus, BusStop busStop)
        {
            Bus = bus;
            BusStop = busStop;
        }

        public abstract void Trigger();

    }


    public class ArriveEvent : Event
    {

        public LineSchedule LineSchedule { get; }

        public ArriveEvent(Bus bus, BusStop busStop) : base(bus, busStop)
        {
        }

        public override void Trigger()
        {
            //doplnit vzdialenost

            Bus.ConsumeBattery();

        }
    }

    public class ChargingEvent : Event
    {
        public int StartTime { get; }
        public int EndTime { get; set; }

        public ChargingEvent(Bus bus, BusStop busStop, int startTime, int EndTime ) : base(bus, busStop)
        {
            this.StartTime = startTime;
            this.EndTime = EndTime;
        }

        public override void Trigger()
        {
            Bus.ChargeBattery(this.EndTime - this.StartTime);
        }
    }
}
