
namespace MachilpebLibrary.Algorithm
{
    

    /*
     * Skratky:
     *      pop = population
     *
     */
    public class MemeticAlgorithm
    {
        //Konstanta
        public static int INDIVIDUAL_COUNT {  get; set; }
        public static int GENERATION_COUNT { get; set; }
        public static int PICKED_COUNT { get; set; } // best picked 

        public static int LOCAL_SEARCH_ITERATION { get; set; }

        public static int Nop = 100; // pocet populacii novych

        private Population _population;

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
            Population currentPop = GenerateInitialPop();
            do
            {
                Population newPop = GenarateNewPop(currentPop);
                currentPop = UpdatePop(currentPop, newPop);
                if (condition) // podmienka ak populacia skonvergovala
                {
                    RestartPop(currentPop);
                }
            } while (condition);
        }

        /*
         * Generate initial population
         * 
         * Metoda sluzi na vygenerovanie zaciatocnej populacie
         * 
         * Vyuziva ku tomu metodu GenerateRandomConfiguration a LocalSearch
         * 
         */
        private Population GenerateInitialPop()
        {
            Population currentPop;
            for (int i = 0; i < currentPop.Size; i++)
            {
                var pop = Individual.GenerateIndividual();
                pop = LocalSearch(pop);
                currentPop.setIndividual(i, pop);
            }

            //return currentPop;
            return null;
        }

        /*
         * Generate new population
         * 
         * Metoda sluzi na vygenerovanie novej populacie
         * Na ktoru sa zoberu rodicia a vytvoria sa deti
         * aplikuju sa operatory
         *      OPERATORY: Selection, Recombination, Mutation, LocalSearch
         * 
         */

        private Population[] GenarateNewPop(Population pCurrentPop)
        {
            Population[Nop] pops;
            pops[0] = pCurrentPop;
            for (int i = 1; i < Nop; i++)
            {
                pops[i] = new Population;
            }

            for (int i = 0; i < Nop; i++)
            {
                Sparent[i] = ExtractFromBuffer(buffer[i - 1], ARITYin[i])
                 Schild[i] = ApplyOperator(op[i], Sparent[i])
                 for (int j = 0; j < ARITYout[i]; i++)
                {
                    pops[j].setIndividual(Schild[j])
                 }
            }

            return pops;
            return null;
        }

        /*
         * Restart population
         * 
         * Metoda sluzi na restartovanie populacie
         * pokial skonvergovala
         * 
         */
        private Population RestartPop(Population pCurrentPop)
        {
            Population newPop;
            int preserved = pCurrentPop.size() * preserve;
            for (int i = 0; i < preserved; i++)
            {
                individual = pCurrentPop.ExtractBest(i);
                newPop.setIndividual(individual, newPop);
            }
            for (int i = preserved; i < pCurrentPop.size(); i++)
            {
                individual = GenerateRandomConfiguration();
                individual = LocalSearch(individual);
                newPop.setIndividual(individual, newPop);
            }
            return newPop;
            return null;
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

                if (actualIndividual.Fitness > newIndividual.Fitness)
                {
                    actualIndividual = newIndividual;
                    i = -1;
                }
            }

            return actualIndividual;

            /*
             * TODO LocalSearch
             * 1. Doriesit podmienku while skoncenia 'TerminationCriterion'
             * 2. Doriesit GenerateNeighbour ako sa bude vytvarat
             * 3. Doriesit podmienku if ako porovnavat 
             */
        }

        private Individual GenerateNeighbour(object currentPop)
        {
            throw new NotImplementedException();
        }

    }
}
