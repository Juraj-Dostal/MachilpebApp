using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MachilpebLibrary
{
    internal sealed class DataReader
    {
        private static DataReader _instance;

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
