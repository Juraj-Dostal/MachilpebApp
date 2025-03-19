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
        public static int POPULATION_SIZE { get; set; }

        private Individual[] _population;

        public Population()
        {
            this._population = new Individual[POPULATION_SIZE];
        }

        public Population(Population population)
        {
            this._population = new Individual[POPULATION_SIZE];
        }

        public void SetIndividual(Individual individual)
        {
            var assigned = false;
            for (int i = 0; i < this._population.Length; i++) 
            {
                if (this._population[i] != null)
                {
                    continue;
                }

                this._population[i] = individual;
                assigned = true;
                break;
            }

            if (!assigned)
            {
                throw new Exception("Individual not assigned!");
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
                weightArray[i] = (double)1 / this._population[i].GetFitnessFun();
                sum += weightArray[i];
            }

            for (int i = 0; i < this._population.Length; i++)
            {
                probalityArray[i] = (this._population[i], weightArray[i] / sum );
            }
            
            var probality = probalityArray.Sum(individual => individual.Item2);

            return probalityArray;
        }

        public Individual[] GetBest(int i)
        {
            var sorted = this._population.OrderBy(individual => individual.GetFitnessFun());

            return sorted.Take(i).ToArray();
        }

        public Individual GetBest()
        { 
            var sorted = this._population.OrderBy(individual => individual.GetFitnessFun());

            return sorted.First();
        }

        public Individual? GetBestAcceptable ()
        {
            return this._population.Where(individual => !individual.IsCancelled()).OrderBy(individual => individual.GetFitnessFun()).FirstOrDefault();
        }
    }
}
