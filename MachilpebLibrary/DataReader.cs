using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MachilpebLibrary
{
    public sealed class DataReader
    {
        private static DataReader _instance;

        private List<Bus> _busList;
        private List<string> _shiftList;
        private List<BusStop> _busStopList;
        private List<LineSchedule> _lineSchedulesList;

        private DataReader() 
        {
            string route = "C:\\Users\\webju\\OneDrive - Žilinská univerzita v Žiline\\Bakalarska praca\\data\\";

            _busList = new List<Bus>();
            _shiftList = new List<string>();
            _busStopList = new List<BusStop>();
            _lineSchedulesList = new List<LineSchedule>();
        

            ReadData(route);

            foreach (var bus in _busList)
            {
                bus.SortSchedules();
            }
        }

        public static DataReader GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DataReader();
            }
            return _instance;
        }

        public string GetStatus()
        {

            var sb = new StringBuilder();

            foreach (var bus in _busList)
            {
                sb.Append(bus.ToString());
                sb.Append("\n");
            }

            sb.Append("\n");

            foreach (var shift in _shiftList)
            {
                sb.Append(shift.ToString());
                sb.Append("\n");
            }


            foreach (var busStop in _busStopList)
            {
                sb.Append(busStop.Name);
                sb.Append("\n");
            }

            return sb.ToString();

        }

        private void ReadData(string route)
        {

            ReadShift(route);
            ReadBusStops(route);
            ReadSergments(route);
            ReadBus(route);
            ReadLineSchedule(route);
            ReadBusStopSchedule(route);

        }

        // metoda nacita TurnusyZoznam
        private void ReadShift(string route)
        {
            string path = route + "TurnusyZoznam.csv";

            var lines = File.ReadAllLines(path).Skip(1);

            foreach (var line in lines)
            {
                var values = line.Split(';');
                _shiftList.Add(values[1].Trim());
            }
        }

        // metoda nacita autobusy
        private void ReadBus(string route)
        {
            string path = route + "TurnusyTyzden.cvr";

            var lines = File.ReadAllLines(path);

            Bus? bus = null;

            foreach (var line in lines)
            {
                var values = line.Split(';');
                var id = int.Parse(values[0]);
                var shift = values[1];

                // ak turnus pre autobus sa nenachadza v zozname turnusov, tak sa preskoci
                if (!_shiftList.Contains(shift))
                {
                    continue;
                }

                // ak autobus ma viac turnusov, tak sa prida turnus k autobusu
                if (bus == null || bus.Id != id)
                {
                    bus = Bus.ReadBus(line);
                    _busList.Add(bus);
                }
                else
                { 
                    bus.AddShift(shift);
                }

            }
        }

        //metoda nacita spoje
        private void ReadLineSchedule(string route)
        {
            string path = route + "Spoje.txt";

            var lines = File.ReadAllLines(path);

            foreach (var line in lines)
            {
                var lineSched = LineSchedule.ReadLineSchedule(line);

                _lineSchedulesList.Add(lineSched);

                // najde autobusy, ktore maju rovnaky turnus a den ako spoj
                var buses = _busList.Where(b => (b.Shift.Contains(lineSched.Shift) && lineSched.Operates.Contains(b.Day))).ToList();

                foreach (var bus in buses)
                { 
                    bus.AddLineSchedule(lineSched);
                }

            }
        }

        // metoda nacita zastavky spojov
        private void ReadBusStopSchedule(string route)
        {
            string path = route + "ZasSpoje.txt";

            var lines = File.ReadAllLines(path);

            var oldId = -1;
            var oldLineId = -1;
            BusStopSchedule? oldBss = null;

            foreach (var line in lines)
            {
                var values = line.Split(',');
                
                if ((values[8].Length == 0 && values[9].Length == 0) || values[9] == "<")
                {
                    continue;
                }
                
                var newId = int.Parse(values[1]);
                var newLineId = int.Parse(values[0]);
                
                var bss = BusStopSchedule.ReadBusStopSchedule(line, _busStopList);

                if (oldId != newId || oldLineId != newLineId)
                {
                    var lineSchedules = _lineSchedulesList.Where(ls => ls.Id == newId && ls.LineId == newLineId).ToList();

                    foreach (var ls in lineSchedules)
                    {
                        ls.AddBusStopSchedule(bss);
                    }

                    oldId = newId;
                    oldLineId = newLineId;
                }
                else
                {
                    if (oldBss == null) 
                    {
                        throw new Exception("Bus stop schedule is null");
                    }
                    oldBss.SetNext(bss);
                }
                
                oldBss = bss;
                
            }
        }


        // metoda nacita zastavky
        private void ReadBusStops(string route)
        {
            string path = route + "Zastavky.csv";

            var lines = File.ReadAllLines(path).Skip(1);

            foreach (var line in lines) 
            { 
                _busStopList.Add(BusStop.ReadBusStop(line));
            }
        }

        // metoda nacita useky
        private void ReadSergments(string route)
        {
            string path = route + "Useky.csv";

            var lines = File.ReadAllLines(path).Skip(1);

            foreach (var line in lines)
            { 
                Segment.ReadSegment(line, _busStopList);
            }

        }

    }
}
