using MachilpebLibrary.Algorithm;
using MachilpebLibrary.Base;
using MachilpebLibrary.Simulation;
using System.Diagnostics;
using System.Text;

namespace MachilpebConsole
{
    internal class Program
    {

        /// <summary>
        /// Memetic algorithm for charging infrastructure location problem for electric buses
        /// </summary>
        /// <param name="populationSize">Number of individuals in population (pcs)</param>
        /// <param name="generationCount">Number of generations of the population (pcs)</param>
        /// <param name="probabilityLocalSearch">Probability of Local Search algorithm execution per individual</param>
        /// <param name="probabilityMutation">Probability of mutation execution per individual</param>
        /// <param name="batteryCharging">Battery charging speed (kWh/min)</param>
        /// <param name="batteryConsumption">Battery consumption (kWh/km) </param>
        /// <param name="batteryCapacity">Battery capacity (kWh)</param>
        /// <param name="priceChargingStation">Cost of building one charging station (Euro)</param>
        /// <param name="priceChargingPoint">Cost of building one charging station (Euro)</param>

        static void Main(int populationSize,
            int generationCount,
            double probabilityLocalSearch,
            double probabilityMutation,
            double batteryCharging = 1.33, 
            double batteryConsumption = 0.8, 
            double batteryCapacity = 140,
            int priceChargingStation = 50000,
            int priceChargingPoint = 2500)
        {
            Bus.BATTERY_CHARGING = batteryCharging;
            Bus.BATTERY_CONSUMPTION = batteryConsumption;
            Bus.BATTERY_CAPACITY = batteryCapacity;

            Individual.PRICE_CHARGING_STATION = priceChargingStation;
            Individual.PRICE_CHARGING_POINT = priceChargingPoint;
            Individual.PRICE_PENALTY = 2 * Individual.PRICE_CHARGING_STATION;

            // treba dat pozor na poradie
            Population.POPULATION_SIZE = populationSize;
            MemeticAlgorithm.GENERATION_COUNT = generationCount;
            MemeticAlgorithm.PROBABILITY_LOCAL_SEARCH = probabilityLocalSearch;
            MemeticAlgorithm.PROBABILITY_MUTATION = probabilityMutation;  
            
            var stopWatch = new Stopwatch();

            stopWatch.Start();

            DataReader dataReader = DataReader.GetInstance();

            var algoritm = new MemeticAlgorithm();

            var bestIndividual = algoritm.MemeticSearch();

            stopWatch.Stop();
            var time = stopWatch.Elapsed;

            var solution = bestIndividual.GetSolution();

            var point = 0;

            foreach (var item in solution)
            {
                Console.WriteLine(item.Item1.Id + " " + item.Item1.Name + " " + item.Item2);
                point += item.Item2;
            }

            Console.WriteLine("Pocet nabijacich stanic: " + solution.Length.ToString());
            Console.WriteLine("Pocet nabijacich bodov: " + point.ToString());
            Console.WriteLine("Cena nakladov: " + bestIndividual.GetObjectiveFun());

            if (bestIndividual.IsCancelled())
            {
                Console.WriteLine(bestIndividual.GetCancelled() + " turnusov nedokoncilo svoju jazdu");
            }
            else
            {
                Console.WriteLine("Vsetky turnusy dokoncili jazdu");
            }

            Console.WriteLine("Time: " + time.TotalSeconds + "s");

        }
    }
}
