using MachilpebLibrary.Base;
using System.ComponentModel.DataAnnotations;

namespace MachilpebLibrary.Algorithm
{
    
    public class MemeticAlgorithm
    {
        //Konstanta
        public static int TERMINATION_CRITERION { get; set; } // pocet generaci od posledneho zlepsenia
        public static int ELITE_COUNT = (int) Math.Round( Population.POPULATION_SIZE * 0.1); // best picked 

        public static int LOCAL_SEARCH_ITERATION = 15;
        public static int PARENTS_COUNT = 2; 

        //Operatos
        public static double PROBABILITY_MUTATION { get; set; } // velmi male
        public static double PROBABILITY_LOCAL_SEARCH { get; set; } // 0 - geneticky a 1 - vzdy sa vykona

        public static double PRESERVE = 0.1; // od <0,1> percento jedincov ktorych si nechame pri restarte

        private Population _population;
        private Random _rnd;

        public MemeticAlgorithm()
        {
            this._population = new Population();
            this._rnd = new Random();
        }


        /*
         * Memetic search  
         * Tato metoda je zaklad memetickeho algoritmu
         * Hlavna metoda
         */
        public Individual MemeticSearch()
        {
            GenerateInitialPop();

            var lastBest = this._population.GetBest();

            for (int i = 0; i < TERMINATION_CRITERION; i++) 
            {
                var pop = this.GenerateNextPopulation();
                this._population = pop;

                var best = this._population.GetBestAcceptable();

                if (best == null)
                {
                    best = this._population.GetBest();
                }

                if (lastBest.GetFitnessFun() < best.GetFitnessFun())
                {
                    lastBest = best;
                    i = -1;
                }

                if (false) // podmienka pre skonvergovanie nezlepsenie fitness funkcie
                {
                    this.RestartPopulation();
                }
            } 

            return this._population.GetBest();
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
         *      OPERATORY: Selection, Crossover, Mutation, LocalSearch
         * 
         */

        private Population GenerateNextPopulation()
        {
            var newPop = new Population(this._population);

            // elitna skupina 
            var elite = this._population.GetBest(ELITE_COUNT);
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

            newPop.SetIndividuals(this._population.GetBest(preserved));

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
                     
            // condition = Opakuj dokym nevyskusas vsetky zmeni
            for (int i = 0; i < BusStop.FINAL_BUSSTOPS.Count ; i++)
            {
                //var newIndividual = actualIndividual.GenerateNeighbour();
                var newIndividual = new Individual(actualIndividual);
                var busStop = BusStop.FINAL_BUSSTOPS[i];

                if (actualIndividual.IsCancelled()) {
                    // pridanie nabijacich bodov
                    var actualPoint = newIndividual.AddChargingPoint(busStop);

                    if (actualPoint == -1)
                    {
                        continue;
                    }
                }
                else
                {
                    // odobratie nabijacich bodov
                    var actualPoint = newIndividual.RemoveChargingPoint(busStop);

                    if (actualPoint == -1)
                    {
                        continue;
                    }
                }

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
