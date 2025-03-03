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

        public void AddIndividual(Individual individual)
        { 
            for (int i = 0; i < this._population.Length; i++) 
            {
                if (this._population[i] != null)
                {
                    continue;
                }

                this._population[i] = individual;
            }

        }




    }
}
