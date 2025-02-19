using MachilpebLibrary.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MachilpebLibrary.Algorithm
{
    public class Individual
    {

        // vektor nabijacich bodov (zastavka, pocet nabijajucich bodov)
        private (BusStop, int)[] _chargingPoint;  


        public Individual()
        {
            var busStop = DataReader.GetInstance().GetBusStop();

            _chargingPoint = new (BusStop, int)[busStop.Count];

            for (int i = 0; i < busStop.Count; i++)
            {
                _chargingPoint[i] = (busStop[i], 0);
            }
        }

        public Individual((BusStop, int)[] chargingPoint)
        {
            _chargingPoint = chargingPoint;
        }

        public int GetChargingPoint(BusStop busStop)
        {
            for (int i = 0; i < _chargingPoint.Length; i++)
            {
                if (_chargingPoint[i].Item1 == busStop)
                {
                    return _chargingPoint[i].Item2;
                }
            }

            throw new ArgumentException("Bus stop not found");
        }

        public void SetChargingPoint(BusStop busStop, int value)
        {
            for (int i = 0; i < _chargingPoint.Length; i++)
            {
                if (_chargingPoint[i].Item1 == busStop)
                {
                    _chargingPoint[i] = (busStop, value);
                    return;
                }
            }

            throw new ArgumentException("Bus stop not found");
        }




    }
}
