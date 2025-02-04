using System;
using System.Collections;
using System.Collections.Generic;
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

    internal class BusStop
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
            Segment? segment = this.Segments.Find(s => s.To == to);
            if (segment == null)
            {
                throw new Exception("Segment not found");
            }

            return segment.Distance;
        }


        public static BusStop ReadBusStop(string line)
        {
            string[] parts = line.Split(';');
            int id = int.Parse(parts[0]);
            string name = parts[1];
            double latitude = double.Parse(parts[2]);
            double longitude = double.Parse(parts[3]);
            return new BusStop(id, name, latitude, longitude);
        }

    }
}
