using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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

            this.DisableChange();

            // parametre sa nesmu zmenit riesenie toho isteho problemu
            Bus.BATTERY_CHARGING = double.Parse(BatteryCharging.Text);
            Bus.BATTERY_CONSUMPTION = double.Parse(BatteryConsumption.Text);
            Bus.BATTERY_CAPACITY = double.Parse(BatteryCapacity.Text);

            Individual.PRICE_CHARGING_STATION = int.Parse(PriceChargingStation.Text);
            Individual.PRICE_CHARGING_POINT = int.Parse(PriceChargingPoint.Text);
            Individual.PRICE_PENALTY = 2 * Individual.PRICE_CHARGING_STATION;

            Population.POPULATION_SIZE = int.Parse(PopulationSize.Text);
            MemeticAlgorithm.TERMINATION_CRITERION = int.Parse(TerminationCriterion.Text);

            MemeticAlgorithm.PROBABILITY_LOCAL_SEARCH = double.Parse(ProbabilityLocalSearch.Text);
            MemeticAlgorithm.PROBABILITY_MUTATION = double.Parse(ProbabilityMutation.Text);

            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                var solutionWindow = new SolutionWindow();

                System.Windows.Threading.Dispatcher.Run();
            }));

            // Set the apartment state
            newWindowThread.SetApartmentState(ApartmentState.STA);
            // Start the thread
            newWindowThread.Start();


            RunAlgorithmButton.IsEnabled = true;
        }

        private void DisableChange()
        {
            BatteryCharging.IsEnabled = false;
            BatteryConsumption.IsEnabled = false;
            BatteryCapacity.IsEnabled = false;
            PriceChargingStation.IsEnabled = false;
            PriceChargingPoint.IsEnabled = false;
            PopulationSize.IsEnabled = false;
            TerminationCriterion.IsEnabled = false;
            ProbabilityLocalSearch.IsEnabled = false;
            ProbabilityMutation.IsEnabled = false;
        }

        // zostava na pozadi proces pokracuje
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            this.Close();
        }
    }
}