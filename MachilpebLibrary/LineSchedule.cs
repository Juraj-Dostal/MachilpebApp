using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachilpebLibrary
{

    /*
     * Trieda LineSchedule
     * 
     * Sluzi na reprezentaciu casoveho harmonogramu linky
     * 
     */

    internal class LineSchedule
    {

        public int Id { get; }
        public int LineId { get; }
        public string shift { get; }
        public BusStopSchedule? BusStopSchedules { get; private set; }

        public LineSchedule(int id, int lineId, string shift)
        {
            this.Id = id;
            this.LineId = lineId;
            this.shift = shift;
        }

        public void AddBusStopSchedule(BusStopSchedule busStopSchedule)
        {
            this.BusStopSchedules = busStopSchedule;
        }

        public static LineSchedule ReadLineSchedule(string line)
        {
            string[] parts = line.Split(',');
            int id = int.Parse(parts[0].Replace("\"", ""));
            int lineId = int.Parse(parts[1].Replace("\"", ""));
            string shift = parts[2].Replace("\"", "").Trim();

            return new LineSchedule(id, lineId, shift);
        }

    }
}
