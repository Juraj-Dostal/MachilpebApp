using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public List<DayOfWeek> Operates { get; private set; }
        public BusStopSchedule? BusStopSchedules { get; private set; }

        public LineSchedule(int id, int lineId, string shift, List<DayOfWeek> operates)
        {
            this.Id = id;
            this.LineId = lineId;
            this.Shift = shift;
            this.Operates = operates;
        }

        public void AddBusStopSchedule(BusStopSchedule busStopSchedule)
        {
            this.BusStopSchedules = busStopSchedule;
        }

        public int GetStartTime()
        {

            if (BusStopSchedules == null)
            {
                throw new Exception("Bus stop schedules not set");
            }

            return BusStopSchedules.Time;
        }

        public override string? ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Id + " line " + LineId + " shift: " + Shift + "\n");

            if (BusStopSchedules != null)
            {
                sb.Append("Bus stop schedules:\n" + BusStopSchedules.ToString() + " \n");
            }

            return sb.ToString();
        }

        public static LineSchedule ReadLineSchedule(string line)
        {
            string[] values = line.Split(',');
            int id = int.Parse(values[1].Replace("\"", ""));
            int lineId = int.Parse(values[0].Replace("\"", ""));
            string shift = values[13].Replace("\"", "").Trim();

            var operates = new List<DayOfWeek>();

            for (int i = 2; values[i].Replace("\"", "").Length != 0; i++)
            { 
                int code = int.Parse(values[i].Replace("\"", ""));

                if (code == 1)
                {
                    operates.AddRange([DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday]);

                }
                else if (code == 2)
                { 
                    operates.AddRange([DayOfWeek.Saturday, DayOfWeek.Sunday]);
                }
                else if (3 < code && code < 12)
                {
                    operates.Add(getDay(code));
                }
            }

            return new LineSchedule(id, lineId, shift, operates);
        }

        public static DayOfWeek getDay(int code)
        {
            switch (code)
            {
                case 7:
                    return DayOfWeek.Monday;
                case 8:
                    return DayOfWeek.Tuesday;
                case 9:
                    return DayOfWeek.Wednesday;
                case 10:
                    return DayOfWeek.Thursday;
                case 11:
                    return DayOfWeek.Friday;
                case 5:
                    return DayOfWeek.Saturday;
                case 4:
                    return DayOfWeek.Sunday;
                default:
                    throw new Exception("Invalid day");
            }
        }

    }
}
