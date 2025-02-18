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
     * TIME je reprezentovane ako pocet minut od polnoci v dni
     */

    public class BusStopSchedule
    {
        public int Sequence { get; private set; }
        public BusStop BusStop { get; }
        public BusStopSchedule? Next { get; private set; }
        public int Time { get; }

        public BusStopSchedule(int sequence, BusStop busStop, int time)
        {
            this.Sequence = sequence;
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

        public override bool Equals(object? obj)
        {
            BusStopSchedule? other = obj as BusStopSchedule;

            if (other == null)
            {
                return false;
            }

            if (this.BusStop != other.BusStop || this.Time != other.Time )
            {
                return false;
            }

            return true;
        }

        public override string? ToString()
        {
            var sb = new StringBuilder();


            sb.Append("     " + Sequence + ". " + BusStop.Name + " " + Time );
            if (Next != null)
            {
                var vzdialenost = this.BusStop.GetDistance(this.Next.BusStop);

                sb.Append(" " + vzdialenost + "\n");
                sb.Append(Next.ToString());
            }

            return sb.ToString();
        }

        public static BusStopSchedule ReadBusStopSchedule(string line, List<BusStop> busStops)
        {
            string[] values = line.Split(',');
                
            var stime = values[8].Length == 0 ? values[9] : values[8];

            var time =  int.Parse(stime.Substring(0, 2)) * 60 + int.Parse(stime.Substring(2, 2));
            int sequence = int.Parse(values[2]);
            int idBusStop = int.Parse(values[3]);

            // specialny pripad pri zastavke 207 autobusova vymenena za 42 Zeleznicna stanica
            idBusStop = idBusStop == 207 ? 42 : idBusStop;

            var busStop = busStops.Find(bs => bs.Id == idBusStop);

            if (busStop == null)
            {
                throw new Exception("Bus stop not found");
            }

            return new BusStopSchedule(sequence ,busStop, time);
        }
    }
}
