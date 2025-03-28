using MachilpebLibrary.Algorithm;
using MachilpebLibrary.Base;
using MachilpebLibrary.Simulation;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MachilpebConsole
{
    internal class Program
    {

        /// <summary>
        /// Memetic algorithm for charging infrastructure location problem for electric buses
        /// </summary>
        /// <param name="populationSize">Number of individuals in population (pcs)</param>
        /// <param name="terminationCriterion">Number of generate generations without better solution (pcs)</param>
        /// <param name="probabilityLocalSearch">Probability of Local Search algorithm execution per individual</param>
        /// <param name="probabilityMutation">Probability of mutation execution per individual</param>
        /// <param name="batteryCharging">Battery charging speed (kWh/min)</param>
        /// <param name="batteryConsumption">Battery consumption (kWh/km) </param>
        /// <param name="batteryCapacity">Battery capacity (kWh)</param>
        /// <param name="priceChargingStation">Cost of building one charging station (Euro)</param>
        /// <param name="priceChargingPoint">Cost of building one charging station (Euro)</param>
        
        static void Main(int populationSize,
            int terminationCriterion,
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
            
            Population.POPULATION_SIZE = populationSize;
            MemeticAlgorithm.GENERATION_COUNT = terminationCriterion;
            MemeticAlgorithm.PROBABILITY_LOCAL_SEARCH = probabilityLocalSearch;
            MemeticAlgorithm.PROBABILITY_MUTATION = probabilityMutation;

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            DataReader dataReader = DataReader.GetInstance();
            var algoritm = new MemeticAlgorithm();
            var bestIndividual = algoritm.MemeticSearch();

            stopWatch.Stop();
            var time = stopWatch.Elapsed;

            Console.Write(bestIndividual[0].GetObjectiveFun() + ";" + bestIndividual[0].GetCancelled());
            Console.Write(bestIndividual[1].GetObjectiveFun() + ";" + bestIndividual[1].GetCancelled() + ";" + time.TotalSeconds );
        }


    }
}
