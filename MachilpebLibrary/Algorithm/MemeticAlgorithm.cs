using System.ComponentModel.DataAnnotations;

namespace MachilpebLibrary.Algorithm
{
    
    public class MemeticAlgorithm
    {
        //Konstanta
        public static int INDIVIDUAL_COUNT {  get; set; }
        public static int GENERATION_COUNT { get; set; }
        public static int PICKED_COUNT { get; set; } // best picked 

        public static int LOCAL_SEARCH_ITERATION { get; set; }
        public static int PARENTS_COUNT { get; set; } // pocet rodicov

        //Operatos
        public static int PROBABILITY_MUTATION { get; set; } // velmi male

        public static double PRESERVE { get; set; } // od <0,1> percento jedincov ktorych si nechame pri restarte

        private Population _population;
        private Random random = new Random();

        public MemeticAlgorithm(Population population)
        {
            _population = population;
        }


        /*
         * Memetic search  
         * Tato metoda je zaklad memetickeho algoritmu
         * Hlavna metoda
         */
        public void MemeticSearch()
        {
            GenerateInitialPop();
            do
            {
                var pop = this.GenerateNextPopulation();
                this._population = pop;
                if (/*condition*/ true) // podmienka pre skonvergovanie nezlepsenie fitness funkcie
                {
                    this.RestartPopulation();
                }
            } while (/*condition*/ true); // podmienka 1.pocet iteracii, 2. pocet restartov 

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
                this._population.AddIndividual(individual);
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
            
            var parent = Selection();
            var children = Crossover(parent);

            newPop.AddIndividual(parent[0]);


            for (int i = 0; i < Population.INDIVIDUAL_COUNT - 1; i++)
            {
                var child = this.Mutation(children[i]);
                child = this.LocalSearch(child);
                newPop.AddIndividual(child);
            }

            return newPop;
        }


        // TODO: Check implemention this method
        private Individual[] Selection()
        {
            var individuals = new Individual[PARENTS_COUNT];
            for (int i = 0; i < PICKED_COUNT; i++)
            {
                individuals[i] = this._population.ExtractBest(i);
            }

            return individuals;
        }

        // TODO: Check implementation this method
        private Individual[] Crossover(Individual[] parents)
        {
            var individuals = new Individual[PARENTS_COUNT];
            var maskSize = parents[0].GetBusStopCount();

            for (int i = 0; i < Population.INDIVIDUAL_COUNT; i++)
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
            if (this.random.NextDouble() < PROBABILITY_MUTATION) 
            {
            
            }

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

            for (int i = 0; i < preserved; i++)
            {
                var individual = this._population.ExtractBest(i);
                newPop.AddIndividual(individual);
            }
            for (int i = preserved; i < Population.INDIVIDUAL_COUNT; i++)
            {
                var individual = Individual.GenerateIndividual();
                individual = this.LocalSearch(individual);
                newPop.AddIndividual(individual);
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
                var newIndividual = this.GenerateNeighbour(actualIndividual);

                if (actualIndividual.GetFitnessFun() > newIndividual.GetFitnessFun())
                {
                    actualIndividual = newIndividual;
                    i = -1;
                }
            }

            return actualIndividual;

            
        }

        // TODO: Implement this method
        private Individual GenerateNeighbour(object currentPop)
        {
            throw new NotImplementedException();
        }

    }
}
