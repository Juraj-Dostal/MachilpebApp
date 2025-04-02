using MachilpebLibrary.Algorithm;
using MachilpebLibrary.Base;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace MachilpebWPF
{
    /// <summary>
    /// Interaction logic for SolutionWindow.xaml
    /// </summary>
    public partial class SolutionWindow : Window
    {
        public SolutionWindow()
        {
            // inicializacia okna
            InitializeComponent();
            this.Show();

            // spustenie algoritmu

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            DataReader dataReader = DataReader.GetInstance();
            var algoritm = new MemeticAlgorithm();
            var bestIndividual = algoritm.MemeticSearch();

            stopWatch.Stop();
            
            var solution = bestIndividual.GetSolution();
            // zobrazenie vysledkov
            var point = 0;

            foreach (var item in solution)
            {
                SolutionTable.Items.Add(new { Id = item.Item1.Id, Name = item.Item1.Name, Point = item.Item2 });
                point += item.Item2;
            }

            ChargingStationBlock.Text = solution.Length.ToString();
            ChargingPointBlock.Text = point.ToString();

            TotalCostsBlock.Text = bestIndividual.GetObjectiveFun().ToString();
            CancalledBlock.Text = bestIndividual.GetCancelled().ToString();

            var time = stopWatch.Elapsed;
            TimeBlock.Text = time.TotalSeconds.ToString();
        }

        private void SolutionWindow_Closing(object sender, CancelEventArgs e)
        {
        }
    }
}
