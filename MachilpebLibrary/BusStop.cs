using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachilpebLibrary
{

    /*
     * Trieda BusStop
     * 
     * Sluzi na reprezentaciu zastavky
     *  
     */

    public class BusStop
    {

        public int Id { get; }
        public string Name { get; }
        public double Latitude { get; }
        public double Longitude { get; }

        // Zoznam usekov, ktore obsahuju usek z tejto zastavky
        private List<Segment> Segments;

        public BusStop(int id, string name, double latitute, double longitute)
        {
            this.Id = id;
            this.Name = name;
            this.Latitude = latitute;
            this.Longitude = longitute;
            this.Segments = new List<Segment>();
        }

        public void AddSegment(Segment segment)
        {
            this.Segments.Add(segment);
        }

        public int GetDistance(BusStop to)
        { 
            if (this == to)
            {
                return 0;
            }

            Segment? segment = this.Segments.Find(s => s.To == to);
            if (segment == null)
            {
                return -1;
                //throw new Exception("Segment not found");
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

    }
}
