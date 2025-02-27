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

            DataReader dataReader = DataReader.GetInstance();

            Individual individual = Individual.GenerateIndividual();

            DiscreteEventSimulation simulation = new DiscreteEventSimulation(individual);

            simulation.simulate();

        }
    }
}
