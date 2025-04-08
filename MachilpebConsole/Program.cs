using MachilpebLibrary.Algorithm;
using MachilpebLibrary.Base;
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
        /// <param name="terminationCriterion">Number of generated generations without better solution (pcs)</param>
        /// <param name="probabilityLocalSearch">Probability of Local Search algorithm execution per individual</param>
        /// <param name="probabilityMutation">Probability of mutation execution per individual</param>
        /// <param name="batteryCharging">Battery charging speed (kWh/min)</param>
        /// <param name="batteryConsumption">Battery consumption (kWh/km) </param>
        /// <param name="batteryCapacity">Battery capacity (kWh)</param>
        /// <param name="priceChargingStation">Cost of building one charging station (Euro)</param>
        /// <param name="priceChargingPoint">Cost of building one charging station (Euro)</param>

        static void Main(int populationSize = 50,
            int terminationCriterion = 25,
            double probabilityLocalSearch = 0.35,
            double probabilityMutation = 0.15,
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
            MemeticAlgorithm.TERMINATION_CRITERION = terminationCriterion;
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

            Console.WriteLine("SOLUTION");
            //Console.WriteLine("Id Busstop | Name busstop | Point (pcs)");
            Console.WriteLine("{0,-10} | {1,-30} | {2,5}", "Id Busstop", "Name busstop", "Point (pcs)");

            foreach (var item in solution)
            {
                //Console.WriteLine(item.Item1.Id + " " + item.Item1.Name + " " + item.Item2);
                Console.WriteLine("{0,-10} | {1,-30} | {2,5}", item.Item1.Id, item.Item1.Name, item.Item2);
                point += item.Item2;
            }

            Console.WriteLine("Number of charging stations: " + solution.Length.ToString());
            Console.WriteLine("Number of charging points: " + point.ToString());
            Console.WriteLine("Cost price: " + bestIndividual.GetObjectiveFun());

            if (bestIndividual.IsCancelled())
            {
                Console.WriteLine("Number of uncompleted tours: " + bestIndividual.GetCancelled());
            }
            else
            {
                Console.WriteLine("All tours have completed the ride");
            }

            Console.WriteLine("Time: " + time.TotalSeconds + "s");
        }
    }
}
