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

        public BusStopSchedule? _firstBusStopSchedule;
        public BusStopSchedule? _lastBusStopSchedule;

        public LineSchedule(int id, int lineId, string shift, List<DayOfWeek> operates)
        {
            Id = id;
            LineId = lineId;
            Shift = shift;
            Operates = operates;
        }

        public void AddBusStopSchedule(BusStopSchedule busStopSchedule)
        {
            if (_firstBusStopSchedule != null)
            {
                busStopSchedule.SetNext(_firstBusStopSchedule);
            }

            _firstBusStopSchedule = busStopSchedule;
        }

        public void AddLastBusStopSchedule(BusStopSchedule busStopSchedule)
        {
            if (_lastBusStopSchedule != null)
            {
                _lastBusStopSchedule.SetNext(busStopSchedule);
            }

            _lastBusStopSchedule = busStopSchedule;
        }

        public BusStopSchedule GetFirstBusStopSchedule()
        {
            return _firstBusStopSchedule ?? throw new Exception("First bus stop schedule not set!");
        }

        public BusStopSchedule GetLastBusStopSchedule()
        {
            return _lastBusStopSchedule ?? throw new Exception("Last bus stop schedule not set!");
        }

        public int GetStartTime()
        {
            return GetFirstBusStopSchedule().Time;
        }

        public int GetEndTime()
        {
            return GetLastBusStopSchedule().Time;
        }

        public int GetDistance()
        {
            int distance = 0;
            var bss = _firstBusStopSchedule;

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
            var thisbss = _firstBusStopSchedule;
            var otherbss = other._firstBusStopSchedule;

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

            if (_firstBusStopSchedule != null)
            {
                sb.Append("Bus stop schedules:\n" + _firstBusStopSchedule.ToString() + " \n");
            }

            return sb.ToString();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, LineId, Shift, Operates, _firstBusStopSchedule, _lastBusStopSchedule);
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
