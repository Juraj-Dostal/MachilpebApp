using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachilpebLibrary
{
    public class Bus
    {                
        public int Id { get; private set; }
        public DayOfWeek Day { get; private set; }
        public List<string> Shift { get; private set; }
        public List<LineSchedule> Schedules { get;  private set; }
        public BusStopSchedule? StartDepo { get; private set; }
        public BusStopSchedule? EndDepo { get; private set; }

        private List<int> Distances; // vzdialenosti medzi presunom na nasledujucu LineSchedule

        public Bus(int id, string shift, DayOfWeek day)
        {
            this.Id = id;
            this.Shift = new List<string>();
            this.Shift.Add(shift);
            this.Day = day;
            this.Schedules = new List<LineSchedule>();
            this.Distances = new List<int>();
        }

        public void AddLineSchedule(LineSchedule lineSchedule)
        {
            Schedules.Add(lineSchedule);
        }

        public void AddShift(string shift)
        {
            this.Shift.Add(shift);
        }

        public void SortSchedules()
        {
            Schedules.Sort((x, y) => x.GetStartTime().CompareTo(y.GetStartTime()));
        }

        public BusStopSchedule GetFirstBusStopInSchedule()
        {
            return Schedules[0].FirstBusStopSchedule;
        }

        public BusStopSchedule GetLastBusStopInSchedule()
        {
            return Schedules[Schedules.Count - 1].LastBusStopSchedule;
        }

        public void AddFirstBusStopInSchedule(BusStopSchedule busStopSchedule)
        {
            Schedules[0].AddBusStopSchedule(busStopSchedule);
        }

        public void AddLastBusStopInSchedule(BusStopSchedule busStopSchedule)
        {
            Schedules[Schedules.Count - 1].AddLastBusStopSchedule(busStopSchedule);
        }

        public void SetDepo(BusStop busStop)
        {
            if (this.StartDepo != null || this.EndDepo != null)
            { 
                throw new Exception("Start or end already set");
            }

            var first = GetFirstBusStopInSchedule();
            var last = GetLastBusStopInSchedule();

            if (first.BusStop.Id == busStop.Id)
            {
                this.StartDepo = first;
            }
            else {
                this.StartDepo = new BusStopSchedule(0, busStop, first.Time - 10);
            }

            if (last.BusStop.Id == busStop.Id)
            {
                this.EndDepo = last;
            }
            else
            {
                this.EndDepo = new BusStopSchedule(0, busStop, last.Time + 10);
            }
            
        }

        public void calculateDistance()
        {

            // Ak startovacie depo je rovnako ako zacina autobus tak da 0
            var prev = this.StartDepo;
            int distance = 0;

            foreach (var schedule in Schedules)
            {
                var actual = schedule.FirstBusStopSchedule;

                distance = prev.BusStop.GetDistance(actual.BusStop);
                Distances.Add(distance);

                prev = schedule.LastBusStopSchedule;
            }

            var last = this.EndDepo;
            distance = prev.BusStop.GetDistance(last.BusStop);
            Distances.Add(distance);
        }


        // metoda skontroluje ci je casovy harmonogram linky spravne zoradeny a opravy duplicity
        public bool IsBusScheduleWell()
        {
            List<LineSchedule> toRemove = new List<LineSchedule>();

            LineSchedule? prev = null;

            foreach (var schedule in Schedules)
            {
                if (prev == null)
                {
                    prev = schedule;
                    continue;
                }

                // ak zacina skor ako konci predosly harmonogram
                if (prev.GetEndTime() <= schedule.GetStartTime())
                {
                    prev = schedule;
                    continue;
                }

                // skontroluje duplicitu
                if (schedule.Equals(prev))
                {
                    toRemove.Add(schedule);
                }
                else
                { 
                    throw new Exception("Invalid schedule\n" + prev.ToString() + "\n" + schedule.ToString());
                }

                prev = schedule;
            }

            foreach (var schedule in toRemove)
            {
                Schedules.Remove(schedule);
            }

            return true;
        }

        public override string? ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Id + ". bus on " + Day.ToString() + " shift: ");

            foreach (var shift in Shift)
            {
                sb.Append(shift + " ");
            }

            sb.Append("\nSchedules:\n");

            if (StartDepo == null || EndDepo == null)
            {
                throw new Exception("Depo not set");
            }

            sb.Append(StartDepo.ToString() + "\n");

            foreach (var schedule in Schedules)
            {
                sb.Append(schedule.ToString());
            }

            sb.Append(EndDepo.ToString() + "\n");

            sb.Append("Distance: ");

            foreach (var distance in Distances)
            {
                sb.Append(distance + " ");
            }

            sb.Append("\n");

            return sb.ToString(); 
        }

        public static Bus ReadBus(string line)
        {
            var values = line.Split(';');
            var id = int.Parse(values[0]);
            var shift = values[1];
            var day = getDay(values[2]);

            return new Bus(id, shift, day);
        }


        private static DayOfWeek getDay(string date)
        {
            int value = int.Parse(date.Substring(date.Length - 1));

            switch (value)
            {
                case 6:
                    return DayOfWeek.Monday;
                case 7:
                    return DayOfWeek.Tuesday;
                case 8:
                    return DayOfWeek.Wednesday;
                case 9:
                    return DayOfWeek.Thursday;
                case 0:
                    return DayOfWeek.Friday;
                case 1:
                    return DayOfWeek.Saturday;
                case 2:
                    return DayOfWeek.Sunday;
            }

            throw new Exception("Invalid day");

        }
    }
    
    public enum DayOfWeek
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }
}
