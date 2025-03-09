using MachilpebLibrary.Algorithm;
using MachilpebLibrary.Base;
using MachilpebLibrary.Simulation;

namespace MachilpebConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Bus.BATTERY_CHARGING = 1.5;
            Bus.BATTERY_CONSUMPTION = 0.8;
            Bus.BATTERY_CAPACITY = 140;
            
            Population.INDIVIDUAL_COUNT = 20;
            
            MemeticAlgorithm.PROBABILITY_LOCAL_SEARCH = 0.35;
            MemeticAlgorithm.PROBABILITY_MUTATION = 0.05;
            MemeticAlgorithm.PRESERVE = 0.1;
            MemeticAlgorithm.PARENTS_COUNT = 5;
            MemeticAlgorithm.LOCAL_SEARCH_ITERATION = 15;
            MemeticAlgorithm.GENERATION_COUNT = 25;
            
            DataReader dataReader = DataReader.GetInstance();

            var algoritm = new MemeticAlgorithm();

            var bestIndividual = algoritm.MemeticSearch();

            Console.WriteLine(bestIndividual.ToString());

        }
    }
}
