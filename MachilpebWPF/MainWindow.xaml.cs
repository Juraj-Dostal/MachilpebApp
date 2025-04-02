using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
        private List<Thread> windowThreads;
        private List<Window> windows;
        private bool _setParameters = false;

        public MainWindow()
        {
            InitializeComponent();
            windowThreads = new List<Thread>();
            windows = new List<Window>();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RunAlgorithmButton.IsEnabled = false;

            if (!_setParameters)
            {
                setParameter();
                _setParameters = true;
            }

            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                var solutionWindow = new SolutionWindow();

                solutionWindow.Closed += (s, args) => System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown();

                windows.Add(solutionWindow);
                System.Windows.Threading.Dispatcher.Run();
            }));

            newWindowThread.SetApartmentState(ApartmentState.STA);
            newWindowThread.Start();

            this.windowThreads.Add(newWindowThread);

            RunAlgorithmButton.IsEnabled = true;
        }

        private void setParameter() 
        {
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
            foreach (var window in windows)
            {
                if (window.Dispatcher != null)
                {
                    window.Dispatcher.InvokeShutdown();
                }
            }

            foreach (var thread in this.windowThreads)
            {
                if (thread.IsAlive)
                {
                    thread.Join();
                }
            }

            windows.Clear();
            windowThreads.Clear();
        }
    }
}