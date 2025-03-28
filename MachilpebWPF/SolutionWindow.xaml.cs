using MachilpebLibrary.Algorithm;
using MachilpebLibrary.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            var solution = bestIndividual[1].GetSolution();

            stopWatch.Stop();

            // zobrazenie vysledkov
            
            var point = 0;
            var solutionTuple = new List<Tuple<int, string, int>>();

            foreach (var item in solution)
            {
                SolutionTable.Items.Add(new { Id = item.Item1.Id, Name = item.Item1.Name, Point = item.Item2 });
                point += item.Item2;
            }

            ChargingStationBlock.Text = solution.Length.ToString();
            ChargingPointBlock.Text = point.ToString();

            TotalCostsBlock.Text = bestIndividual[1].GetObjectiveFun().ToString();
            CancalledBlock.Text = bestIndividual[1].GetCancelled().ToString();

            var time = stopWatch.Elapsed;
            TimeBlock.Text = time.TotalSeconds.ToString();
        }

        private void SolutionWindow_Closing(object sender, CancelEventArgs e)
        {
            this.Close();
        }
    }
}
