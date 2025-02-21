using System.Text;

namespace MachilpebLibrary.Base
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
            Sequence = sequence;
            BusStop = busStop;
            Time = time;
        }

        // vrati vzdialenost na dalsiu zastavku
        public int GetDistanceToNext()
        { 
            if (Next == null)
            {
                throw new Exception("Next not set");
            }

            return BusStop.GetDistance(Next.BusStop);
        }

        public void SetNext(BusStopSchedule next)
        {
            if (Next != null)
            {
                throw new Exception("Next already set");
            }
            Next = next;
        }

        public override bool Equals(object? obj)
        {
            BusStopSchedule? other = obj as BusStopSchedule;

            if (other == null)
            {
                return false;
            }

            if (BusStop != other.BusStop || Time != other.Time)
            {
                return false;
            }

            return true;
        }

        public override string? ToString()
        {
            var sb = new StringBuilder();


            sb.Append("     " + Sequence + ". " + BusStop.Name + " " + Time);
            if (Next != null)
            {
                var vzdialenost = BusStop.GetDistance(Next.BusStop);

                sb.Append(" " + vzdialenost + "\n");
                sb.Append(Next.ToString());
            }

            return sb.ToString();
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(Sequence, BusStop, Next, Time);
        }
        
        public static BusStopSchedule ReadBusStopSchedule(string line, List<BusStop> busStops)
        {
            string[] values = line.Split(',');

            var stime = values[8].Length == 0 ? values[9] : values[8];

            var time = int.Parse(stime.Substring(0, 2)) * 60 + int.Parse(stime.Substring(2, 2));
            int sequence = int.Parse(values[2]);
            int idBusStop = int.Parse(values[3]);

            // specialny pripad pri zastavke 207 autobusova vymenena za 42 Zeleznicna stanica
            idBusStop = idBusStop == 207 ? 42 : idBusStop;

            var busStop = busStops.Find(bs => bs.Id == idBusStop);

            if (busStop == null)
            {
                throw new Exception("Bus stop not found");
            }

            return new BusStopSchedule(sequence, busStop, time);
        }

        
    }
}
