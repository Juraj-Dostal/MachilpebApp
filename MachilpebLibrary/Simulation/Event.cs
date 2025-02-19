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
    public class Event
    {

        public Bus Bus { get; }
        public int Time { get; }
        public BusStop BusStop { get; }

        public Event(Bus bus, int time, BusStop busStop)
        {
            Bus = bus;
            Time = time;
            BusStop = busStop;
        }





    }


    public class ArriveEvent : Event
    {
        public ArriveEvent(Bus bus, int time, BusStop busStop) : base(bus, time, busStop)
        {
        }
    }

    public class ChargingEvent : Event
    {
        public ChargingEvent(Bus bus, int time, BusStop busStop) : base(bus, time, busStop)
        {
        }
    }
}
