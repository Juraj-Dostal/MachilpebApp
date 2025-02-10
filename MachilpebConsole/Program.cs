using MachilpebLibrary;

namespace MachilpebConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {

            DataReader dataReader = DataReader.GetInstance();

            Console.WriteLine(dataReader.GetStatus());


        }
    }
}
