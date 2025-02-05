using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachilpebLibrary
{
    internal class Segment
    {

        public BusStop From { get; }
        public BusStop To { get; }
        public int Distance { get; }

        public Segment(BusStop from, BusStop to, int distance)
        {
            this.From = from;
            this.To = to;
            this.Distance = distance;
        }

        public static Segment ReadSegment(string line, List<BusStop> busStops)
        {
            string[] parts = line.Split(';');
            int fromId = int.Parse(parts[1]);
            int toId = int.Parse(parts[2]);
            int distance = int.Parse(parts[3]);
            BusStop? from = busStops.Find(bs => bs.Id == fromId);
            BusStop? to = busStops.Find(bs => bs.Id == toId);

            if (from == null || to == null)
            {
                throw new Exception("Bus stop not found");
            }

            var segment = new Segment(from, to, distance);

            from.AddSegment(segment);

            return segment;
        }

    }
}
