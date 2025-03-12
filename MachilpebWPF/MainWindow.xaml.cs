using System.Diagnostics;
using System.Text;
using System.Windows;
using MachilpebLibrary.Algorithm;
using MachilpebLibrary.Base;


namespace MachilpebWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RunAlgorithmButton.IsEnabled = false;

            Bus.BATTERY_CHARGING = double.Parse(BatteryCharging.Text);
            Bus.BATTERY_CONSUMPTION = double.Parse(BatteryConsumption.Text);
            Bus.BATTERY_CAPACITY = double.Parse(BatteryCapacity.Text);

            Individual.PRICE_CHARGING_STATION = int.Parse(PriceChargingStation.Text);
            Individual.PRICE_CHARGING_POINT = int.Parse(PriceChargingPoint.Text);
            Individual.PRICE_PENALTY = 2 * Individual.PRICE_CHARGING_STATION;

            Population.POPULATION_SIZE = int.Parse(PopulationSize.Text);
            MemeticAlgorithm.GENERATION_COUNT = int.Parse(GenerationCount.Text);
            
            MemeticAlgorithm.PROBABILITY_LOCAL_SEARCH = double.Parse(ProbabilityLocalSearch.Text);
            MemeticAlgorithm.PROBABILITY_MUTATION = double.Parse(ProbabilityMutation.Text);


            var stopWatch = new Stopwatch();

            stopWatch.Start();

            DataReader dataReader = DataReader.GetInstance();

            var algoritm = new MemeticAlgorithm();

            var bestIndividual = algoritm.MemeticSearch();
            
            var solution = bestIndividual.GetSolution();

            stopWatch.Stop();

            var sb = new StringBuilder();
            var point = 0;

            foreach (var item in solution)
            {
                sb.Append(item.Item1.Id + " " + item.Item1.Name + " " + item.Item2 + "\n");
                point += item.Item2;
            }

            solutionBlock.Text = sb.ToString();

            ChargingStationBlock.Text = solution.Length.ToString();
            ChargingPointBlock.Text = point.ToString();

            TotalCostsBlock.Text = bestIndividual.GetObjectiveFun().ToString();

            var time = stopWatch.Elapsed;
            TimeBlock.Text = time.TotalSeconds.ToString();

            RunAlgorithmButton.IsEnabled = true;

        }
    }
}