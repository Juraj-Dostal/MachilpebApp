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
     * Abstractna trieda Event
     * 
     * Potomkovia:
     * 
     *  ArriveEvent - reprezentuje prichod autobusu na konecnu
     *  ChargingEvent - reprezentuje ukoncenie nabijanie autobusu
     *  RelocationEvent - reprezentuje presun autobusu na dalsi LineSchedule 
     *  ! Nabijanie moze nastat pred presunom alebo po presune na dalsi LineSchedule
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

        public ArriveEvent(Bus bus, BusStop busStop, LineSchedule lineSchedule) : base(bus, busStop)
        {
            LineSchedule = lineSchedule;
        }

        public override void Trigger()
        {
            Bus.ConsumeBattery(LineSchedule.GetDistance());
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

    public class RelocationEvent : Event
    {
        public BusStop Destination { get; }

        public RelocationEvent(Bus bus, BusStop busStop, BusStop destination) : base(bus, busStop)
        {
            Destination = destination;
        }

        public override void Trigger()
        {
            Bus.ConsumeBattery(BusStop.GetDistance(Destination));
        }
    }
}
