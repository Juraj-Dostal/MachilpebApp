using MachilpebLibrary.Base;
using System.ComponentModel.DataAnnotations;

namespace MachilpebLibrary.Algorithm
{
    
    public class MemeticAlgorithm
    {
        //Konstanta
        public static int GENERATION_COUNT { get; set; }
        public static int ELITE_COUNT = (int) Math.Round( Population.POPULATION_SIZE * 0.1); // best picked 

        public static int LOCAL_SEARCH_ITERATION = 15;
        public static int PARENTS_COUNT = 2; 

        //Operatos
        public static double PROBABILITY_MUTATION { get; set; } // velmi male
        public static double PROBABILITY_LOCAL_SEARCH { get; set; } // 0 - geneticky a 1 - vzdy sa vykona

        public static double PRESERVE = 0.1; // od <0,1> percento jedincov ktorych si nechame pri restarte

        private Population _population;
        private Random _rnd = new();

        public MemeticAlgorithm()
        {
            _population = new Population();
        }


        /*
         * Memetic search  
         * Tato metoda je zaklad memetickeho algoritmu
         * Hlavna metoda
         */
        public Individual MemeticSearch()
        {
            GenerateInitialPop();
            for (int i = 0; i < GENERATION_COUNT; i++) 
            {
                var pop = this.GenerateNextPopulation();
                this._population = pop;

                if (false) // podmienka pre skonvergovanie nezlepsenie fitness funkcie
                {
                    this.RestartPopulation();
                }
            } 

            return this._population.ExtractBest(1)[0];
        }

        /*
         * Generate initial population
         * 
         * Metoda sluzi na vygenerovanie zaciatocnej populacie
         *  
         */
        private void GenerateInitialPop()
        {
            for (int i = 0; i < Population.POPULATION_SIZE; i++)
            {
                var individual = new Individual();
                individual = this.LocalSearch(individual);
                this._population.SetIndividual(individual);
            }

        }

        /*
         * Generate new population
         * 
         * Metoda sluzi na vygenerovanie novej dalsej populacie
         * Na ktoru sa zoberu rodicia a vytvoria sa deti
         * aplikuju sa operatory
         *      OPERATORY: Selection, Recombination, Mutation, LocalSearch
         * 
         */

        private Population GenerateNextPopulation()
        {
            var newPop = new Population(this._population);

            // elitna skupina 
            var elite = this._population.ExtractBest(ELITE_COUNT);
            newPop.SetIndividuals(elite);

            var children = Crossover();

            for (int i = 0; i < children.Length; i++)
            {
                var child = children[i];

                if (this._rnd.NextDouble() < PROBABILITY_MUTATION)
                {
                    child = this.Mutation(child);
                }
                if (this._rnd.NextDouble() < PROBABILITY_LOCAL_SEARCH)
                {
                    child = this.LocalSearch(child);
                }

                newPop.SetIndividual(child);
            }

            return newPop;
        }


        // TODO: Check implemention this method
        // treba vyberat podla pravdepodobnosti, kde lepsie maju vacsiu pravdepodobnost
        // treba urobit prevratenu hodnotu 
        // rulettet-wheel selection

        private Individual[] Selection((Individual, double)[] probalityArray)
        {
            var individuals = new Individual[PARENTS_COUNT];

            for (int i = 0; i < PARENTS_COUNT; i++)
            {
                var rnd = this._rnd.NextDouble();

                double from = 0;

                for (int j = 0; j < probalityArray.Length; j++)
                {
                    double to = from + probalityArray[j].Item2;

                    if (from <= rnd && rnd < to)
                    {
                        if (individuals.Contains(probalityArray[j].Item1))
                        {
                            i--;
                        }
                        else
                        {
                            individuals[i] = probalityArray[j].Item1;
                        }

                        break;
                    }

                    from = to;
                } 
            }

            return individuals;
        }

        // TODO: Check implementation this method
        private Individual[] Crossover()
        {
            var probability = this._population.GetProbalityArray();

            var individuals = new Individual[Population.POPULATION_SIZE - ELITE_COUNT];
            var maskSize = BusStop.FINAL_BUSSTOPS.Count;

            for (int i = 0; i < Population.POPULATION_SIZE - ELITE_COUNT; i++)
            {
                var parents = this.Selection(probability);

                var mask = this.GenerateMask(maskSize);

                individuals[i] = new Individual(parents[0], parents[1], mask);
            }

            return individuals;
        }

        private bool[] GenerateMask(int size)
        {
            var mask = new bool[size];

            for (int i = 0; i < mask.Length; i++)
            {
                mask[i] = this._rnd.NextDouble() < 0.5;
            }

            return mask;
        }

        // TODO: Implement this method
        private Individual Mutation(Individual individual)
        {
            individual.Mutate();

            return individual;

        }

        /*
         * Restart population
         * 
         * Metoda sluzi na restartovanie populacie
         * pokial skonvergovala
         * 
         */
        private Population RestartPopulation()
        {
            var newPop = new Population();

            var preserved = (int) Math.Round( Population.POPULATION_SIZE * PRESERVE);

            newPop.SetIndividuals(this._population.ExtractBest(preserved));

            //for (int i = 0; i < preserved; i++)
            //{
            //    var individual = this._population.ExtractBest(i);
            //    newPop.SetIndividuals(individual);
            //}
            for (int i = preserved; i < Population.POPULATION_SIZE; i++)
            {
                var individual = new Individual();
                individual = this.LocalSearch(individual);
                newPop.SetIndividual(individual);
            }

            return newPop;
        }

        /*
         * Local search algorithm
         * Tento algoritmus sa vyuziva pri vytvoreni zaciatocnej populacie a pri vytvoreni novej populacie ako operator
         */
        private Individual LocalSearch(Individual individual)
        {
            var actualIndividual = individual;
                     
            // condition = Opakuje sa vopred urcenych m pocet iteracii, kde sa nenaslo zlepsenie v poslednych m iteraciach
            for (int i = 0; i < LOCAL_SEARCH_ITERATION ; i++)
            {
                var newIndividual = actualIndividual.GenerateNeighbour();

                if (actualIndividual.GetFitnessFun() > newIndividual.GetFitnessFun())
                {
                    actualIndividual = newIndividual;
                    i = -1;
                }
            }

            return actualIndividual;
        }


    }
}
