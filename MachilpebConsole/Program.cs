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

            var populationSizeArray = new[] { 50, 30, 75, 100, 150 };
            var terminationCriterionArray = new[] { 10, 15, 25, 50, 100 };
            var probabilityLocalSearchArray = new[] { 0.35, 0, 0.2, 0.5, 0.75, 1 };
            var probabilityMutationArray = new[] { 0.05, 0, 0.1, 0.15, 0.2, 0.4 };


            if (populationSize == -1)
            {
                foreach (var item in populationSizeArray)
                {
                    File.AppendAllText("./data(populationSize).csv", "Population size;Termination criterion;Probability local search;Probability mutation\n");
                    FindBestParameters("./data(populationSize).csv", item, terminationCriterion, probabilityLocalSearch, probabilityMutation);
                }
            }

            if (terminationCriterion == -1)
            {
                foreach (var item in terminationCriterionArray)
                {
                    File.AppendAllText("./data(terminationCriterion).csv", "Population size;Termination criterion;Probability local search;Probability mutation\n");
                    FindBestParameters("./data(terminationCriterion).csv", populationSize, item, probabilityLocalSearch, probabilityMutation);
                }
            }

            if (probabilityLocalSearch == -1)
            {
                foreach (var item in probabilityLocalSearchArray)
                {
                    File.AppendAllText("./data(probabilityLocalSearch).csv", "Population size;Termination criterion;Probability local search;Probability mutation\n");
                    FindBestParameters("./data(probabilityLocalSearch).csv", populationSize, terminationCriterion, item, probabilityMutation);
                }
            }

            if (probabilityMutation == -1)
            {
                foreach (var item in probabilityMutationArray)
                {
                    File.AppendAllText("./data(probabilityMutation).csv", "Population size;Termination criterion;Probability local search;Probability mutation\n");
                    FindBestParameters("./data(probabilityMutation).csv", populationSize, terminationCriterion, probabilityLocalSearch, item);
                }
            }
        }

        // priemerna ucelova funkcia, min ucelova funkcia, priemer pocet nedokoncenych turnusov, priemer doba pocitania
        private static void FindBestParameters(string path ,int populationSize, int terminationCritetion, double probabilityLocalSearch, double probabilityMutation)
        {
            
            Population.POPULATION_SIZE = populationSize;
            MemeticAlgorithm.GENERATION_COUNT = terminationCritetion;
            MemeticAlgorithm.PROBABILITY_LOCAL_SEARCH = probabilityLocalSearch;
            MemeticAlgorithm.PROBABILITY_MUTATION = probabilityMutation;

            File.AppendAllText(path, ";;;\n");
            File.AppendAllText(path, populationSize + ";" + terminationCritetion + ";" + probabilityLocalSearch + ";" + probabilityMutation + "\n");
            File.AppendAllText(path, ";;;\n");

            for (int i = 0; i < 10; i++)
            {
                var stopWatch = new Stopwatch();

                stopWatch.Start();

                DataReader dataReader = DataReader.GetInstance();

                var algoritm = new MemeticAlgorithm();

                var bestIndividual = algoritm.MemeticSearch();

                stopWatch.Stop();

                var time = stopWatch.Elapsed;

                File.AppendAllText(path, bestIndividual.GetObjectiveFun() + ";" + bestIndividual.GetCancelled() + ";" + time.TotalSeconds + "\n");

            }

        }
    }
}
