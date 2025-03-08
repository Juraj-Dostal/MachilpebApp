using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachilpebLibrary.Algorithm
{
    public class Population
    {
        // Konstanta
        public static int INDIVIDUAL_COUNT { get; set; }

        private Individual[] _population;

        public Population()
        {
            this._population = new Individual[INDIVIDUAL_COUNT];
        }

        public Population(Population population)
        {
            this._population = new Individual[INDIVIDUAL_COUNT];
            for (int i = 0; i < population._population.Length; i++)
            {
                this._population[i] = new Individual(population._population[i]);
            }
        }

        public void SetIndividual(Individual individual)
        { 
            for (int i = 0; i < this._population.Length; i++) 
            {
                if (this._population[i] != null)
                {
                    continue;
                }

                this._population[i] = individual;
                break;
            }
        }

        public void SetIndividuals(Individual[] individuals)
        {
            for (int i = 0; i < individuals.Length; i++)
            {
                this.SetIndividual(individuals[i]);
            }
        }

        public (Individual, double)[] GetProbalityArray()
        {
            var weightArray = new double[this._population.Length];
            var probalityArray = new (Individual, double)[this._population.Length];

            var sum = 0.0;

            for (int i = 0; i < this._population.Length; i++)
            {
                weightArray[i] = (double)1 / (this._population[i].GetFitnessFun() / 10000);
                sum += weightArray[i];
            }

            for (int i = 0; i < this._population.Length; i++)
            {
                probalityArray[i] = (this._population[i], weightArray[i] / sum );
            }

            return probalityArray;
        }

        //TODO: Implement this method
        internal Individual[] ExtractBest(int i)
        {
            var sorted = this._population.OrderBy(individual => individual.GetFitnessFun());

            return sorted.Take(i).ToArray();
        }
    }
}
