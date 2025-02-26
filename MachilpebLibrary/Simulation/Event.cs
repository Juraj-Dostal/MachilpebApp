using MachilpebLibrary.Algorithm;
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
        public int Priority { get; }
    

        public Event(Bus bus, BusStop busStop, int priority)
        {
            this.Bus = bus;
            this.BusStop = busStop;
            this.Priority = priority;
        }

        public abstract void Trigger();

        public abstract override string? ToString();
    }

    public class ArriveEvent : Event
    {
        public LineSchedule LineSchedule { get; }

        public ArriveEvent(Bus bus, BusStop busStop, int priority, LineSchedule lineSchedule) : base(bus, busStop, priority)
        {
            this.LineSchedule = lineSchedule;
        }

        public override void Trigger()
        {
            this.Bus.ConsumeBattery(this.LineSchedule.GetDistance());
        }

        public override string? ToString()
        {
            return this.Priority + " | ArriveEvent: " + this.Bus.ToString() + " BusStop: " + this.BusStop.Name + "\n";
        }
    }

    public class ChargingEvent : Event
    {
        private Individual _individual;

        public int StartTime { get; }
        public int EndTime { get; set; }

        public ChargingEvent(Bus bus, BusStop busStop, Individual individual , int priority, int startTime, int endTime ) : base(bus, busStop, priority)
        {
            this.StartTime = startTime;
            this.EndTime = endTime;
            this._individual = individual;
        }

        public override void Trigger()
        {
            this.Bus.ChargeBattery(this.EndTime - this.StartTime);
            this._individual.FreeChargingPoint(this.BusStop);
        }

        public override string? ToString()
        {
            return this.Priority + " | ChargingEvent " + this.Bus.ToString() + " BusStop: " + this.BusStop.Name + "Time" + this.StartTime + "-" + this.EndTime + "\n";
        }
    }

    public class RelocationEvent : Event
    {
        public BusStop Destination { get; }

        public RelocationEvent(Bus bus, BusStop busStop, int priority, BusStop destination) : base(bus, busStop, priority)
        {
            this.Destination = destination;
        }

        public override void Trigger()
        {
            this.Bus.ConsumeBattery(this.BusStop.GetDistance(this.Destination));
        }

        public override string? ToString()
        {
            return this.Priority + " | RelocationEvent: " + this.Bus.ToString() + " BusStop: " + this.BusStop.Name + " Destination: " + this.Destination.Name + "\n";
        }
    }
}
