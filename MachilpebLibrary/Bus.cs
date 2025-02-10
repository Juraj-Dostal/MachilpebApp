using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachilpebLibrary
{
    internal class Bus
    {
                
        public int Id { get; private set; }
        public DayOfWeek Day { get; private set; }
        public List<string> Shift { get; private set; }
        public List<LineSchedule> schedules { get;  private set; }

        public Bus(int id, string shift, DayOfWeek day)
        {
            this.Id = id;
            this.Shift = new List<string>();
            this.Shift.Add(shift);
            this.Day = day;
            this.schedules = new List<LineSchedule>();
        }

        public void AddLineSchedule(LineSchedule lineSchedule)
        {
            schedules.Add(lineSchedule);
        }

        public void AddShift(string shift)
        {
            this.Shift.Add(shift);
        }

        public override string? ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Id + ". bus on " + Day.ToString() + " shift: ");

            foreach (var shift in Shift)
            {
                sb.Append(shift + " ");
            }

            sb.Append("\n Schedules:");

            foreach (var schedule in schedules)
            {
                sb.Append(schedule.ToString());
            }

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
    
    enum DayOfWeek
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
