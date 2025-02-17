using MachilpebLibrary;

namespace MachilpebConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {

            DataReader dataReader = DataReader.GetInstance();

            // vystup do suboru
            File.WriteAllText("C:\\Users\\webju\\OneDrive - Žilinská univerzita v Žiline\\Bakalarska praca\\data\\outputV2.txt", dataReader.GetStatus());

        }
    }
}
