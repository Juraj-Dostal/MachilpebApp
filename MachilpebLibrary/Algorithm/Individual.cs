using MachilpebLibrary.Base;
using MachilpebLibrary.Simulation;
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
        // Konstanta
        public static int PRICE_CHARGING_STATION { get; set; }// €
        public static int PRICE_CHARGING_POINT { get; set; } // €
        public static int PRICE_PENALTY { get; set; } // €

        private Random _rnd = new();

        //string route = "C:\\Users\\webju\\OneDrive - Žilinská univerzita v Žiline\\Bakalarska praca\\data\\Log_Simulate_Individual.txt";

        // vektor nabijacich bodov (zastavka, pocet nabijajucich bodov)
        private (BusStop, int)[] _chargingPoint;
        private (BusStop, int)[] _usedChargingPoint;

        // atribute potrebne pre algoritmus
        private int _cancelled = 0; // pocet neuskutocnenych turnusov

        public Individual()
        {
            var busStop = BusStop.FINAL_BUSSTOPS;

            this._chargingPoint = new (BusStop, int)[busStop.Count];
            this._usedChargingPoint = new (BusStop, int)[busStop.Count];

            for (int i = 0; i < busStop.Count; i++)
            {
                this._chargingPoint[i] = (busStop[i], 0);
                this._usedChargingPoint[i] = (busStop[i], 0);
            }

            // inicializacia nabijacich bodov 
            this.InitChargingPoint();

            // simulacia jedinca
            this.SimulateIndividual();
        }

        public Individual(Individual individual)
        {

            this._chargingPoint = new (BusStop, int)[individual._chargingPoint.Length];
            this._usedChargingPoint = new (BusStop, int)[individual._chargingPoint.Length];

            for (int i = 0; i < individual._chargingPoint.Length; i++)
            {
                this._chargingPoint[i] = (individual._chargingPoint[i].Item1, individual._chargingPoint[i].Item2);
                this._usedChargingPoint[i] = (individual._usedChargingPoint[i].Item1, 0);
            }

            this.SimulateIndividual();
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

            this.SimulateIndividual();
        }

        public Individual(Individual firstParent, Individual secondParent, bool[] mask)
        {
            this._chargingPoint = new (BusStop, int)[firstParent._chargingPoint.Length];
            this._usedChargingPoint = new (BusStop, int)[firstParent._chargingPoint.Length];

            for (int i = 0; i < firstParent._chargingPoint.Length; i++)
            {
                if (mask[i])
                {
                    this._chargingPoint[i] = (firstParent._chargingPoint[i].Item1, firstParent._chargingPoint[i].Item2);
                }
                else
                {
                    this._chargingPoint[i] = (secondParent._chargingPoint[i].Item1, secondParent._chargingPoint[i].Item2);
                }

                this._usedChargingPoint[i] = (firstParent._chargingPoint[i].Item1, 0);
            }

            this.SimulateIndividual();
        }


        /*
         * Tato metoda sluzi pre Local Search
         * 
         * Snazi sa zlepsovat jedinca tak, ze zmeni pocet nabijacich bodov na jednej zastavke
         * 
         * individual je validny tak uberat nabijacie pointy
         * individual nie je validny tak pridavat nabijacie pointy
         */

        public Individual GenerateNeighbour()
        {
            var copy = new (BusStop, int)[this._chargingPoint.Length];
            Array.Copy(this._chargingPoint, copy, this._chargingPoint.Length);

            if (this._cancelled < 0)
            {
                var cp = copy.Where((individual, point) => point > 0);
                var bs = cp.ElementAt(this._rnd.Next(0, cp.Count()));
                bs.Item2--;
            }
            else
            {
                var cp = copy.Where((individual, point) => point == 0);
                var bs = cp.ElementAt(this._rnd.Next(0, cp.Count()));
                bs.Item2++;
            }

            return new Individual(copy);
        }

        public void Mutate()
        {
            var i = this._rnd.Next(0, this._chargingPoint.Length);

            if (this._chargingPoint[i].Item2 == 0)
            {
                this._chargingPoint[i].Item2++;
            }
            else
            {
                this._chargingPoint[i].Item2--;
            }

            this.SimulateIndividual();
        }

        public int GetFitnessFun()
        {
            var price = this.GetObjectiveFun();

            price += this._cancelled * PRICE_PENALTY;

            return price;
        }

        public int GetObjectiveFun()
        {
            var price = 0;

            for (int i = 0; i < this._chargingPoint.Length; i++)
            {
                if (this._chargingPoint[i].Item2 > 0)
                {
                    price += PRICE_CHARGING_STATION + this._chargingPoint[i].Item2 * PRICE_CHARGING_POINT;
                }
            }

            return price; 
        }

        public int GetBusStopCount()
        {
            return this._chargingPoint.Length;
        }

        public int GetChargingPoint(BusStop busStop)
        {
            return this._chargingPoint[this.FindBusStop(busStop)].Item2;
        }

        public int GetCancelled()
        {
            return this._cancelled;
        }

        public bool IsCancelled()
        {
            return this._cancelled > 0;
        }

        public (BusStop, int)[] GetSolution()
        {
            return this._chargingPoint.Where(c => c.Item2 > 0).ToArray();
        }

        public int AddChargingPoint(BusStop busStop)
        {
            return this._chargingPoint[this.FindBusStop(busStop)].Item2++;
        }

        public int RemoveChargingPoint(BusStop busStop)
        {
            if (this._chargingPoint[this.FindBusStop(busStop)].Item2 == 0)
            {
                return -1;
            }

            return this._chargingPoint[this.FindBusStop(busStop)].Item2--;
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
            var count = this._rnd.Next(0, this._chargingPoint.Length);

            for (int i = 0; i < count;)
            {
                var bs = this._rnd.Next(0, this._chargingPoint.Length);

                if (this._chargingPoint[bs].Item2 != 0)
                {
                    continue;
                }

                this._chargingPoint[bs].Item2 = this._rnd.Next(1, 3);

                i++;

                //File.AppendAllText(route, this._chargingPoint[bs].Item1.Id + " " + this._chargingPoint[bs].Item1.Name + " " + this._chargingPoint[bs].Item2 + "\n");
            }
        }

        private void SimulateIndividual()
        {
            var simulation = new DiscreteEventSimulation(this);
            this._cancelled = simulation.simulate();
        }

        public override string? ToString()
        {
            var sb = new StringBuilder();

            foreach (var c in this._chargingPoint)
            {
                sb.Append(c.Item1.Id + " " + c.Item1.Name + " " + c.Item2 + "\n");
            }

            return sb.ToString();

        }

        public override bool Equals(object? obj)
        {
            return obj is Individual individual && EqualityComparer<(BusStop, int)[]>.Default.Equals(_chargingPoint, individual._chargingPoint);
        }
       
    }
}
