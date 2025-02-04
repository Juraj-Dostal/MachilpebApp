using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MachilpebLibrary
{
    /*
     * Trieda BusStopSchedule
     * 
     * Sluzi na reprezentaciu casoveho harmonogramu zastavky
     *  
     *  TIME je reprezentovane ako pocet minut od polnoci v dni
     */

    internal class BusStopSchedule
    {
        public BusStop BusStop { get; }
        public BusStopSchedule? Next { get; private set; }
        public int Time { get; }

        public BusStopSchedule(BusStop busStop, int time)
        {
            this.BusStop = busStop;
            this.Time = time;
        }

        public void SetNext(BusStopSchedule next)
        {
            if (this.Next != null)
            {
                throw new Exception("Next already set");
            }
            this.Next = next;
        }

        public static BusStopSchedule ReadBusStopSchedule(string text, List<BusStop> busStops)
        {
            string[] lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            BusStopSchedule? first = null;
            BusStopSchedule? old = null;

            foreach (string line in lines)
            {
                string stime;
                int time;

                string[] parts = line.Split(',');
                
                if (parts[8].Length == 0 && parts[9].Length == 0)
                {
                    break;
                }
                if (parts[9] == "<")
                { 
                    continue;
                }

                stime = parts[8].Length == 0 ? parts[9] : parts[8];

                time =  int.Parse(stime.Substring(0, 2)) * 60 + int.Parse(stime.Substring(2, 2));
                
                int idBusStop = int.Parse(parts[3]);

                BusStop busStop = busStops.Find(bs => bs.Id == idBusStop);

                if (busStop == null)
                {
                    throw new Exception("Bus stop not found");
                }

                BusStopSchedule bss = new BusStopSchedule(busStop, time);

                if (first == null)
                {
                    first = bss;
                }

                if (old != null)
                {
                    old.SetNext(bss);
                }

                old = bss;
            }

            return first;
        }
    }
}
