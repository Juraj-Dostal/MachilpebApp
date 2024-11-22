namespace MachilpebLibrary
{
    /*
     * Skratky:
     *      pop = population
     *
     */
    public class MemeticAlgorithm
    {
        static int Nop = 100; // pocet populacii novych


        /*
         * Local search algorithm
         * Tento algoritmus sa vyuziva pri vytvoreni zaciatocnej populacie a pri vytvoreni novej populacie ako operator
         */
        private static Population LocalSearch(Population pCurrentPop) 
        {
            Population currentPop = pCurrentPop;
            Population newPop;

            while (condition) {
                newPop = GenerateNeighbour(currentPop);
                if (newPop.Fitness > currentPop.Fitness) {
                    currentPop = newPop;
                }
            }
            return currentPop;

            /*
             * 1. Doriesit podmienku while skoncenia 'TerminationCriterion'
             * 2. Doriesit GenerateNeighbour ako sa bude vytvarat
             * 3. Doriesit podmienku if ako porovnavat 
             */
        }

        /*
         * Memetic search  
         * Tato metoda je zaklad memetickeho algoritmu
         * Hlavna metoda
         */
        public static void MemeticSearch()
        { 
            Population currentPop = GenerateInitialPop();
            do {
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
        private static Population GenerateInitialPop()
        {
            Population currentPop;
            for (int i = 0; i < currentPop.Size; i++)
            {
                pop = GenerateRandomConfiguration();
                pop = LocalSearch(pop);
                currentPop.setIndividual(i, pop);
            }

            return currentPop;
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

        private static Population[] GenarateNewPop(Population pCurrentPop)
        {
            Population[Nop] pops;
            pops[0] = pCurrentPop;
            for (int i = 1; i < Nop; i++)
            {
                pops[i] = new Population;
            }
            
            for (int i = 0; i < Nop; i++)
            {
                // Sparent[i] = ExtractFromBuffer(buffer[i-1], ARITYin[i])
                // Schild[i] = ApplyOperator(op[i] ,Sparent[i])
                // for(int j = 0; j < ARITYout[i]; i++) {
                //      pops[j].setIndividual(Schild[j])
                // }
            }

            return pops;
        }

        /*
         * Restart population
         * 
         * Metoda sluzi na restartovanie populacie
         * pokial skonvergovala
         * 
         */
        private static Population RestartPop(Population pCurrentPop) 
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
        }

    }
}
