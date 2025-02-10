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
        public string Shift { get; }
        public BusStopSchedule? BusStopSchedules { get; private set; }

        public LineSchedule(int id, int lineId, string shift)
        {
            this.Id = id;
            this.LineId = lineId;
            this.Shift = shift;
        }

        public void AddBusStopSchedule(BusStopSchedule busStopSchedule)
        {
            this.BusStopSchedules = busStopSchedule;
        }

        public override string? ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Id + ". line " + LineId + " shift: " + Shift + "\n");

            if (BusStopSchedules != null)
            {
                sb.Append("Bus stop schedules:" + BusStopSchedules.ToString() + " \n");
            }

            return sb.ToString();
        }

        public static LineSchedule ReadLineSchedule(string line)
        {
            string[] parts = line.Split(',');
            int id = int.Parse(parts[1].Replace("\"", ""));
            int lineId = int.Parse(parts[0].Replace("\"", ""));
            string shift = parts[12].Replace("\"", "").Trim();

            return new LineSchedule(id, lineId, shift);
        }

    }
}
