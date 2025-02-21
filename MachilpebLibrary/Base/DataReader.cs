using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MachilpebLibrary.Base
{
    public sealed class DataReader
    {
        private static DataReader _instance;

        private List<Bus> _busList;
        private List<string> _shiftList;
        private List<BusStop> _busStopList;
        private List<LineSchedule> _lineSchedulesList;
        private List<Segment> _segmentsList;

        private DataReader()
        {
            string route = "C:\\Users\\webju\\OneDrive - Žilinská univerzita v Žiline\\Bakalarska praca\\data\\";

            _busList = [];
            _shiftList = [];
            _busStopList = [];
            _lineSchedulesList = [];
            _segmentsList = [];

            Bus.BATTERY_CAPACITY = 140;
            Bus.BATTERY_CONSUMPTION = 0.8;
            Bus.BATTERY_CHARGING = 1;

            ReadData(route);

            EditAndControlData();
        }

        public static DataReader GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DataReader();
            }
            return _instance;
        }

        public ImmutableList<BusStop> GetBusStop()
        {
            return _busStopList.ToImmutableList();
        }

        public ImmutableList<Bus> GetBuses()
        {
            return _busList.ToImmutableList();
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

        private void EditAndControlData()
        {

            foreach (var bus in _busList)
            {
                bus.SortSchedules();
            }

            var badBus = _busList.Where(b => b.GetSchedules().Count == 0).ToList();

            foreach (var bus in badBus)
            {
                _busList.Remove(bus);
            }

            var depo = _busStopList.Find(b => b.Id == 112);
            if (depo == null)
            {
                throw new Exception("Depo not found");
            }

            foreach (var bus in _busList)
            {
                bus.SetDepo(depo);
            }

            foreach (var bus in _busList)
            {
                bus.calculateDistance();
            }

            foreach (var bus in _busList)
            {
                bus.IsBusScheduleWell();
            }
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
                var buses = _busList.Where(b => b.GetShift().Contains(lineSched.Shift) && lineSched.Operates.Contains(b.Day)).ToList();

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
            List<LineSchedule>? lineSchedules = null;

            foreach (var line in lines)
            {
                var values = line.Split(',');

                if (values[8].Length == 0 && values[9].Length == 0 || values[9] == "<")
                {
                    continue;
                }

                var newId = int.Parse(values[1]);
                var newLineId = int.Parse(values[0]);

                var bss = BusStopSchedule.ReadBusStopSchedule(line, _busStopList);

                if (oldId != newId || oldLineId != newLineId)
                {

                    if (lineSchedules != null && oldBss != null)
                    {
                        setLastBusSTopSchedule(lineSchedules, oldBss);
                    }

                    lineSchedules = _lineSchedulesList.Where(ls => ls.Id == newId && ls.LineId == newLineId).ToList();

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

            if (lineSchedules != null && oldBss != null)
            {
                setLastBusSTopSchedule(lineSchedules, oldBss);
            }
        }

        private void setLastBusSTopSchedule(List<LineSchedule> lineSchedules, BusStopSchedule bss)
        {
            foreach (var ls in lineSchedules)
            {
                ls.AddLastBusStopSchedule(bss);
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
            string path = route + "UsekyEdit.txt"; //Edit(doplnene)

            var lines = File.ReadAllLines(path).Skip(1);

            foreach (var line in lines)
            {
                var segment = Segment.ReadSegment(line, _busStopList);
                _segmentsList.Add(segment);
            }

        }

    }
}
