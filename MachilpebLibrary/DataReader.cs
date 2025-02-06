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
        private List<string> _turnusList;
        private List<BusStop> _busStopList;

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

        private void ReadData(String path)
        { 
            string pathTT = path + "TurnusyZoznam.cvr";
            string pathS = path + "Spoje.txt";
            string pathSZ = path + "Zastavky.txt";



        
        }

        // metoda nacita TurnusyZoznam
        private void ReadList()
        {
            string path = "TurnusyZoznam.csv";

            var lines = File.ReadAllLines(path);

            foreach (var line in lines)
            {
                var values = line.Split(';');
                _turnusList.Add(values[1]);
            }
        }

        // metoda nacita autobusy
        private void ReadBus()
        {
            // nacitanie dat

            string path = "TurnusyTyzden.cvr";
            //string pathS =  "Spoje.txt";
            //string pathSZ =  "Zastavky.txt";

            var lines = File.ReadAllLines(path);
            //var linesS = File.ReadAllLines(pathS);
            //var linesSZ = File.ReadAllLines(pathSZ);

            Bus bus = null;

            foreach (var lineTT in lines)
            {
                var values = lineTT.Split(';');
                var id = int.Parse(values[0]);
                var turnus = values[1];
                var day = getDay(values[2]);
                if (bus == null || bus.Id != id )
                {
                    bus = new Bus(id, day);
                }
                _busList.Add(bus);


            }
        }

        private void ReadLineSchedule()
        {
            string path = "Spoje.txt";

            var lines = File.ReadAllLines(path);

            foreach (var line in lines)
            {
                LineSchedule.ReadLineSchedule(line);
            }
        }

        private void ReadBusStopSchedule(LineSchedule lineSchedule)
        {
            string path = "Zastavky.txt";

            var lines = File.ReadAllLines(path);

            foreach (var line in lines)
            {
                BusStopSchedule.ReadBusStopSchedule(line, _busStopList);
            }
        }

        private DayOfWeek getDay(string date)
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


        // metoda nacita zastavky
        private void ReadBusStops()
        {
            string path = "Zastavky.csv";

            var lines = File.ReadAllLines(path);

            foreach (var line in lines) 
            { 
                _busStopList.Add(BusStop.ReadBusStop(line));
            }
        }

        // metoda nacita useky
        private void ReadSergments()
        {
            string path = "Useky.csv";

            var lines = File.ReadAllLines(path);

            foreach (var line in lines)
            { 
                Segment.ReadSegment(line, _busStopList);
            }

        }







    }
}
