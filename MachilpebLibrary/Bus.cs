using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachilpebLibrary
{
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


    internal class Bus
    {
                
        public int Id { get; private set; }
        public DayOfWeek Day { get; private set; }
        public List<LineSchedule> schedules { get;  private set; }

        public Bus(int id, DayOfWeek day)
        {
            this.Id = id;
            this.Day = day;
        }

        public void AddLineSchedule(LineSchedule lineSchedule)
        {
            schedules.Add(lineSchedule);
        }


    }
}
