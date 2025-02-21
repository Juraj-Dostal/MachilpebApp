using System;
using System.Text;

namespace MachilpebLibrary.Base
{

    /*
     * Trieda LineSchedule
     * 
     * Sluzi na reprezentaciu casoveho harmonogramu linky
     * 
     */

    public class LineSchedule
    {

        public int Id { get; }
        public int LineId { get; }
        public string Shift { get; }
        public List<DayOfWeek> Operates { get; private set; }
        public BusStopSchedule? FirstBusStopSchedule { get; private set; }
        public BusStopSchedule? LastBusStopSchedule { get; private set; }

        public LineSchedule(int id, int lineId, string shift, List<DayOfWeek> operates)
        {
            Id = id;
            LineId = lineId;
            Shift = shift;
            Operates = operates;
        }

        public void AddBusStopSchedule(BusStopSchedule busStopSchedule)
        {
            if (FirstBusStopSchedule != null)
            {
                busStopSchedule.SetNext(FirstBusStopSchedule);
            }

            FirstBusStopSchedule = busStopSchedule;
        }

        public void AddLastBusStopSchedule(BusStopSchedule busStopSchedule)
        {
            if (LastBusStopSchedule != null)
            {
                LastBusStopSchedule.SetNext(busStopSchedule);
            }

            LastBusStopSchedule = busStopSchedule;
        }

        public int GetStartTime()
        {
            if (FirstBusStopSchedule == null)
            {
                throw new Exception("Bus stop schedules not set");
            }

            return FirstBusStopSchedule.Time;
        }

        public int GetEndTime()
        {
            if (LastBusStopSchedule == null)
            {
                throw new Exception("Bus stop schedules not set");
            }

            return LastBusStopSchedule.Time;
        }

        public int GetDistance()
        {
            int distance = 0;
            var bss = FirstBusStopSchedule;

            while (bss != null)
            {
                distance += bss.GetDistanceToNext();
                bss = bss.Next;
            }

            return distance;

        }

        public override bool Equals(object? obj)
        {
            LineSchedule? other = obj as LineSchedule;

            if (other == null)
            {
                return false;
            }

            // skontroluje ci sa jedna o rovnaku linku a zmenu
            //if (this.LineId != other.LineId && this.Shift != other.Shift)
            //{
            //    return false;
            //}

            // skontroluje ci sa jedna o rovnaky casovy harmonogram
            var thisbss = FirstBusStopSchedule;
            var otherbss = other.FirstBusStopSchedule;

            while (thisbss != null && otherbss != null)
            {
                if (!thisbss.Equals(otherbss))
                {
                    return false;
                }

                thisbss = thisbss.Next;
                otherbss = otherbss.Next;
            }

            return true;
        }

        public override string? ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Id + " line " + LineId + " shift: " + Shift + "\n");

            if (FirstBusStopSchedule != null)
            {
                sb.Append("Bus stop schedules:\n" + FirstBusStopSchedule.ToString() + " \n");
            }

            return sb.ToString();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, LineId, Shift, Operates, FirstBusStopSchedule, LastBusStopSchedule);
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
