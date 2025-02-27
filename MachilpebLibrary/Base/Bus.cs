using System;
using System.Collections.Immutable;
using System.Text;

namespace MachilpebLibrary.Base
{
    public class Bus
    {
        // konstanty
        public static double BATTERY_CAPACITY { get; set; } // kWh
        public static double BATTERY_CONSUMPTION { get; set; } // kWh/km
        public static double BATTERY_CHARGING { get; set; } // kWh/min
        public static double RELOCATION_SPEED = 40; // km/h priemerna ryclost na presun autobusu

        // identifikacia busu
        public int Id { get; private set; }
        public DayOfWeek Day { get; private set; }

        public BusStopSchedule? _startDepo;
        public BusStopSchedule? _endDepo;

        private readonly List<int> _distances; // vzdialenosti medzi presunom na nasledujucu LineSchedule
        private readonly List<string> _shift;
        private readonly List<LineSchedule> _schedules;

        // atributy ku simulacii
        private double _battery; // kWh 

        public Bus(int id, string shift, DayOfWeek day)
        {
            Id = id;
            _shift = [shift];
            Day = day;
            _schedules = [];
            _distances = [];
        }

        public void AddLineSchedule(LineSchedule lineSchedule)
        {
            _schedules.Add(lineSchedule);
        }

        public void AddShift(string shift)
        {
            _shift.Add(shift);
        }

        public void SortSchedules()
        {
            _schedules.Sort((x, y) => x.GetStartTime().CompareTo(y.GetStartTime()));
        }

        // metoda vracia nasledujuci harmonogram linky
        // ak je parameter null tak vrati prvy harmonogram
        // ak je parameter posledny harmonogram tak vrati null
        public LineSchedule? GetNextLineSchedule(LineSchedule? lineSchedule = null)
        {
            if (lineSchedule == null)
            {
                return _schedules.First();
            }

            var index = _schedules.IndexOf(lineSchedule) + 1;

            if (index == _schedules.Count)
            {
                return null;
            }

            return this._schedules[index];
        }

        public BusStopSchedule? GetDistanceFromDepo()
        {
            if (this.GetStartDepo().Equals(GetFirstBusStopInSchedule()))
            { 
                return null;
            }

            return this._startDepo;
        }

        public BusStopSchedule? GetDistanceToDepo()
        {
            if (this.GetEndDepo().Equals(GetLastBusStopInSchedule()))
            {
                return null;
            }

            return this._endDepo;
        }

        public BusStopSchedule GetStartDepo()
        {
            if (this._startDepo == null)
            {
                throw new Exception("Start depo not set");
            }
            return this._startDepo;
        }

        public BusStopSchedule GetEndDepo()
        {
            if (this._endDepo == null)
            {
                throw new Exception("End depo not set");
            }
            return this._endDepo;
        }

        public ImmutableList<LineSchedule> GetSchedules()
        {
            return _schedules.ToImmutableList();
        }

        public ImmutableList<string> GetShift()
        {
            return _shift.ToImmutableList();
        }

        public BusStopSchedule GetFirstBusStopInSchedule()
        {
            var busStopSchedule = _schedules[0]._firstBusStopSchedule;

            if (busStopSchedule == null)
            {
                throw new Exception("Last bus stop not set");
            }

            return busStopSchedule;
        }

        public BusStopSchedule GetLastBusStopInSchedule()
        {
            var busStopSchedule = _schedules[_schedules.Count - 1]._lastBusStopSchedule;

            if (busStopSchedule == null)
            {
                throw new Exception("Last bus stop not set");
            }

            return busStopSchedule;
        }

        public void AddFirstBusStopInSchedule(BusStopSchedule busStopSchedule)
        {
            _schedules[0].AddBusStopSchedule(busStopSchedule);
        }

        public void AddLastBusStopInSchedule(BusStopSchedule busStopSchedule)
        {
            _schedules[_schedules.Count - 1].AddLastBusStopSchedule(busStopSchedule);
        }

        // metoda nabije bateria podla casu nabijania
        public void ChargeBattery(int time)
        {
            _battery += (int)(time * BATTERY_CHARGING);
            if (_battery > BATTERY_CAPACITY)
            {
                _battery = BATTERY_CAPACITY;
            }
        }

        // metoda nastavi bateriu na plnu kapacitu
        public void ChargeBattery()
        {
            _battery = BATTERY_CAPACITY;
        }

        // metoda spotrebuje bateriu podla prejdenych metrov
        public void ConsumeBattery(int meters)
        {
            var kilometers = meters / 1000;
            _battery -= kilometers * BATTERY_CONSUMPTION;
            if (_battery < 0 )
            {
                _battery = 0;
            }
        }

        public void SetDepo(BusStop busStop)
        {
            if (_startDepo != null || _endDepo != null)
            {
                throw new Exception("Start or end already set");
            }

            var first = GetFirstBusStopInSchedule();
            var last = GetLastBusStopInSchedule();

            if (first.BusStop.Id == busStop.Id)
            {
                _startDepo = first;
            }
            else
            {
                _startDepo = new BusStopSchedule(0, busStop, first.Time - 10);
            }

            if (last.BusStop.Id == busStop.Id)
            {
                _endDepo = last;
            }
            else
            {
                _endDepo = new BusStopSchedule(0, busStop, last.Time + 10);
            }

        }

        public void calculateDistance()
        {

            // Ak startovacie depo je rovnako ako zacina autobus tak da 0
            var prev = this.GetStartDepo();

            int distance;

            foreach (var schedule in _schedules)
            {
                var actual = schedule._firstBusStopSchedule;

                if (actual == null)
                {
                    throw new Exception("First bus stop not set");
                }

                distance = prev.BusStop.GetDistance(actual.BusStop);
                _distances.Add(distance);

                prev = schedule._lastBusStopSchedule;

                if (prev == null)
                {
                    throw new Exception("Last bus stop not set");
                }
            }

            var last = this.GetEndDepo();

            distance = prev.BusStop.GetDistance(last.BusStop);
            _distances.Add(distance);
        }


        // metoda skontroluje ci je casovy harmonogram linky spravne zoradeny a opravy duplicity
        public bool IsBusScheduleWell()
        {
            List<LineSchedule> toRemove = [];

            LineSchedule? prev = null;

            foreach (var schedule in _schedules)
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
                _schedules.Remove(schedule);
            }

            return true;
        }

        public override string? ToString()
        {
            //var sb = new StringBuilder();

            //sb.Append(Id + ". bus on " + Day.ToString() + " shift: ");

            //foreach (var shift in _shift)
            //{
            //    sb.Append(shift + " ");
            //}

            //sb.Append("\nSchedules:\n");

            //sb.Append(this.GetStartDepo().ToString() + "\n");

            //foreach (var schedule in _schedules)
            //{
            //    sb.Append(schedule.ToString());
            //}

            //sb.Append(this.GetEndDepo().ToString() + "\n");

            //sb.Append("Distance: ");

            //foreach (var distance in _distances)
            //{
            //    sb.Append(distance + " ");
            //}

            //sb.Append("\n");

            //return sb.ToString();

            return Id + ". bus on " + Day.ToString() + " battery: " + _battery;
        }

        public static Bus ReadBus(string line)
        {
            var values = line.Split(';');
            var id = int.Parse(values[0]);
            var shift = values[1];
            var day = getDay(values[2]);

            return new Bus(id, shift, day);
        }

        public static int GetRelocationTime(int relocationDistance)
        {
            return (int)Math.Round(relocationDistance * RELOCATION_SPEED * 1000 / 60); 
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
