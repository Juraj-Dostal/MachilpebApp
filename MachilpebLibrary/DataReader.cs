using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MachilpebLibrary
{
    internal sealed class DataReader
    {
        private static DataReader _instance;

        private List<Bus> _busList;
        private List<string> _shiftList;
        private List<BusStop> _busStopList;
        private List<LineSchedule> _lineSchedulesList;

        private DataReader() 
        {
        
        }

        public static DataReader GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DataReader();
            }
            return _instance;
        }

        private void ReadData(string route)
        { 



        
        }

        // metoda nacita TurnusyZoznam
        private void ReadShift(string route)
        {
            string path = route + "TurnusyZoznam.csv";

            var lines = File.ReadAllLines(path);

            foreach (var line in lines)
            {
                var values = line.Split(';');
                _shiftList.Add(values[1]);
            }
        }

        // metoda nacita autobusy
        private void ReadBus(string route)
        {
            string path = route + "TurnusyTyzden.cvr";

            var lines = File.ReadAllLines(path);

            Bus bus = null;

            foreach (var line in lines)
            {
                var values = line.Split(';');
                var id = int.Parse(values[0]);
                var shift = values[1];

                // ak turnus pre autobus sa nenachadza v zozname turnusov, tak sa preskoci
                if (!_shiftList.Contains(shift) )
                {
                    continue;
                }

                // ak autobus ma viac turnusov , tak sa prida turnus k autobusu
                if (bus == null || bus.Id != id)
                {
                    bus = Bus.ReadBus(line);
                }
                else
                { 
                    bus.AddShift(shift);
                }
                _busList.Add(bus);

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

                // najde autobusy, ktore maju rovnaky turnus 
                var buses = _busList.Where(b => b.Shift.Contains(lineSched.shift)).ToList();

                foreach (var bus in buses)
                { 
                    bus.AddLineSchedule(lineSched);
                }

            }
        }

        // metoda nacita zastavky spojov
        private void ReadBusStopSchedule(string route)
        {
            string path = route + "Zastavky.txt";

            var lines = File.ReadAllLines(path);

            var oldId = -1;
            LineSchedule? lineSchedule = null;
            BusStopSchedule? oldBss = null;

            foreach (var line in lines)
            {
                var values = line.Split(',');
                
                if (values[8].Length == 0 && values[9].Length == 0)
                {
                    break;
                }
                if (values[9] == "<")
                {
                    continue;
                }
                
                var newId = int.Parse(values[1]);
                
                var bss = BusStopSchedule.ReadBusStopSchedule(line, _busStopList);

                if (oldId != newId)
                {
                    lineSchedule = _lineSchedulesList.Find(ls => ls.Id == newId);

                    if (lineSchedule == null)
                    {
                        throw new Exception("Line schedule not found");
                    }

                    lineSchedule.AddBusStopSchedule(bss);

                    oldId = newId;
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

            var lines = File.ReadAllLines(path);

            foreach (var line in lines) 
            { 
                _busStopList.Add(BusStop.ReadBusStop(line));
            }
        }

        // metoda nacita useky
        private void ReadSergments(string route)
        {
            string path = route + "Useky.csv";

            var lines = File.ReadAllLines(path);

            foreach (var line in lines)
            { 
                Segment.ReadSegment(line, _busStopList);
            }

        }

    }
}
