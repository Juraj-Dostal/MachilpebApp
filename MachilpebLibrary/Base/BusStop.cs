using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachilpebLibrary.Base
{

    /*
     * Trieda BusStop
     * 
     * Sluzi na reprezentaciu zastavky
     *  
     */

    public class BusStop
    {
        public static List<BusStop> FINAL_BUSSTOPS { get; set; }

        public int Id { get; }
        public string Name { get; }
        public double Latitude { get; }
        public double Longitude { get; }

        // Zoznam usekov, ktore obsahuju usek z tejto zastavky
        private readonly List<Segment> _segments;

        public BusStop(int id, string name, double latitute, double longitute)
        {
            Id = id;
            Name = name;
            Latitude = latitute;
            Longitude = longitute;
            _segments = [];
        }

        public void AddSegment(Segment segment)
        {
            _segments.Add(segment);
        }

        public int GetDistance(BusStop to)
        {
            if (this == to)
            {
                return 0;
            }

            Segment? segment = _segments.Find(s => s.To == to);
            if (segment == null)
            {
                throw new Exception("Segment not found\n From:" + this.Id + " " + this.Name + " To:" + to.Id + " " + to.Name + "\n");
            }

            return segment.Distance;
        }


        public static BusStop ReadBusStop(string line)
        {
            string[] values = line.Split(';');
            int id = int.Parse(values[0]);
            string name = values[1];
            double latitude = double.Parse(values[2], CultureInfo.InvariantCulture);
            double longitude = double.Parse(values[3], CultureInfo.InvariantCulture);
            return new BusStop(id, name, latitude, longitude);
        }

        public static void SetFinalBusStops(List<LineSchedule> lineSchedules)
        {
            FINAL_BUSSTOPS = new List<BusStop>();
            foreach (var lineSchedule in lineSchedules)
            {
                var firstBusStop = lineSchedule.GetFirstBusStopSchedule().BusStop;
                var lastBusStop = lineSchedule.GetLastBusStopSchedule().BusStop;

                if (!FINAL_BUSSTOPS.Contains(firstBusStop))
                {
                    FINAL_BUSSTOPS.Add(firstBusStop);
                }
                else if (!FINAL_BUSSTOPS.Contains(lastBusStop))
                {
                    FINAL_BUSSTOPS.Add(lastBusStop);
                }

            }

        }
    }
}
