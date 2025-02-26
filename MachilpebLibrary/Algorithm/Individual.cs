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
        string route = "C:\\Users\\webju\\OneDrive - Žilinská univerzita v Žiline\\Bakalarska praca\\data\\Log_Simulate_Individual.txt";

        // vektor nabijacich bodov (zastavka, pocet nabijajucich bodov)
        private (BusStop, int)[] _chargingPoint;
        private (BusStop, int)[] _usedChargingPoint;

        public Individual()
        {
            var busStop = DataReader.GetInstance().GetBusStop();

            this._chargingPoint = new (BusStop, int)[busStop.Count];
            this._usedChargingPoint = new (BusStop, int)[busStop.Count];

            for (int i = 0; i < busStop.Count; i++)
            {
                this._chargingPoint[i] = (busStop[i], 0);
                this._usedChargingPoint[i] = (busStop[i], 0);
            }

            // inicializacia nabijacich bodov !!Docasne
            this.InitChargingPoint();
        }

        public Individual((BusStop, int)[] chargingPoint)
        {
            this._chargingPoint = chargingPoint;

            var busStop = DataReader.GetInstance().GetBusStop();

            this._usedChargingPoint = new (BusStop, int)[chargingPoint.Length];

            for (int i = 0; i < chargingPoint.Length; i++)
            {
                this._usedChargingPoint[i] = (chargingPoint[i].Item1, 0);
            }
        }

        public int GetChargingPoint(BusStop busStop)
        {
            return this._chargingPoint[this.FindBusStop(busStop)].Item2;
        }

        public void SetChargingPoint(BusStop busStop, int value)
        {
            this._chargingPoint[FindBusStop(busStop)] = (busStop, value);
        }

        public bool IsChargingPointFree(BusStop busStop)
        {
            var i = FindBusStop(busStop);

            return (this._chargingPoint[i].Item2 - this._usedChargingPoint[i].Item2) != 0;
        }

        public bool UseChargingPoint(BusStop busStop)
        {
            var i = FindBusStop(busStop);

            if (this._chargingPoint[i].Item2 - this._usedChargingPoint[i].Item2 > 0)
            {
                this._usedChargingPoint[i].Item2++;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool FreeChargingPoint(BusStop busStop)
        {
            var i = FindBusStop(busStop);

            if (this._usedChargingPoint[i].Item2 > 0)
            {
                this._usedChargingPoint[i].Item2--;
                return true;
            }
            else
            {
                return false;
            }
        }

        private int FindBusStop(BusStop busStop)
        {
            for (int i = 0; i < this._chargingPoint.Length; i++)
            {
                if (this._chargingPoint[i].Item1 == busStop)
                {
                    return i;
                }
            }

            throw new ArgumentException("Bus stop not found in Individual");
        }

        private void InitChargingPoint()
        {
            var rnd = new Random();

            for (int i = 0; i < 20;)
            {
                var bs = rnd.Next(0, this._chargingPoint.Length);

                if (this._chargingPoint[bs].Item2 != 0)
                {
                    continue;
                }

                this._chargingPoint[bs].Item2 = rnd.Next(1, 3);

                i++;

                File.AppendAllText(route, this._chargingPoint[bs].Item1.Id + " " + this._chargingPoint[bs].Item1.Name + " " + this._chargingPoint[bs].Item2 + "\n");
            }
        }

    }
}
