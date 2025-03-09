using System.ComponentModel.DataAnnotations;

namespace MachilpebLibrary.Algorithm
{
    
    public class MemeticAlgorithm
    {
        //Konstanta
        public static int GENERATION_COUNT { get; set; }
        public static int ELITE_COUNT = (int) Math.Round( Population.INDIVIDUAL_COUNT * 0.1); // best picked 

        public static int LOCAL_SEARCH_ITERATION { get; set; }
        public static int PARENTS_COUNT { get; set; } // pocet rodicov

        //Operatos
        public static double PROBABILITY_MUTATION { get; set; } // velmi male
        public static double PROBABILITY_LOCAL_SEARCH { get; set; } // 0 - geneticky a 1 - vzdy sa vykona

        public static double PRESERVE { get; set; } // od <0,1> percento jedincov ktorych si nechame pri restarte

        private Population _population;
        private Random random = new();

        public MemeticAlgorithm()
        {
            _population = new Population();
        }


        /*
         * Memetic search  
         * Tato metoda je zaklad memetickeho algoritmu
         * Hlavna metoda
         */
        public String MemeticSearch()
        {
            GenerateInitialPop();
            for (int i = 0; i < GENERATION_COUNT; i++) // podmienka 1.pocet iteracii, 2. pocet restartov
            {
                var pop = this.GenerateNextPopulation();
                this._population = pop;

                if (false) // podmienka pre skonvergovanie nezlepsenie fitness funkcie
                {
                    this.RestartPopulation();
                }
            } 

            var bestIndividual = this._population.ExtractBest(1)[0];

            return bestIndividual.ToString();

        }

        /*
         * Generate initial population
         * 
         * Metoda sluzi na vygenerovanie zaciatocnej populacie
         *  
         */
        private void GenerateInitialPop()
        {
            for (int i = 0; i < Population.INDIVIDUAL_COUNT; i++)
            {
                var individual = Individual.GenerateIndividual();
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

            var parent = Selection();
            var children = Crossover(parent);

            for (int i = 0; i < children.Length; i++)
            {
                var child = children[i];

                if (this.random.NextDouble() < PROBABILITY_MUTATION)
                {
                    child = this.Mutation(child);
                }
                if (this.random.NextDouble() < PROBABILITY_LOCAL_SEARCH)
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

        private Individual[] Selection()
        {
            var individuals = new Individual[PARENTS_COUNT];
            var probalityArray = this._population.GetProbalityArray();

            for (int i = 0; i < PARENTS_COUNT; i++)
            {
                var rnd = this.random.NextDouble();

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
        private Individual[] Crossover(Individual[] parents)
        {
            var individuals = new Individual[Population.INDIVIDUAL_COUNT - ELITE_COUNT];
            var maskSize = parents[0].GetBusStopCount();

            for (int i = 0; i < Population.INDIVIDUAL_COUNT - ELITE_COUNT; i++)
            {
                var mask = this.GenerateMask(maskSize);

                var firstParent = this.random.Next(0, PARENTS_COUNT);
                int secondParent;

                do {
                    secondParent = this.random.Next(0, PARENTS_COUNT);
                } while (firstParent == secondParent);
                
                individuals[i] = new Individual(parents[firstParent], parents[secondParent], mask);
            }

            return individuals;
        }

        private bool[] GenerateMask(int size)
        {
            var mask = new bool[size];

            for (int i = 0; i < mask.Length; i++)
            {
                mask[i] = this.random.NextDouble() < 0.5;
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

            var preserved = (int) Math.Round( Population.INDIVIDUAL_COUNT * PRESERVE);

            newPop.SetIndividuals(this._population.ExtractBest(preserved));

            //for (int i = 0; i < preserved; i++)
            //{
            //    var individual = this._population.ExtractBest(i);
            //    newPop.SetIndividuals(individual);
            //}
            for (int i = preserved; i < Population.INDIVIDUAL_COUNT; i++)
            {
                var individual = Individual.GenerateIndividual();
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
