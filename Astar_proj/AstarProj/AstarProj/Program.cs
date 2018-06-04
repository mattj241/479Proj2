using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstarProj
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Main loop, keeps running different words until the user types EXIT or closes the program
            while (true)
            {
                bool testForImpossible = false;
                bool timeout = false;
                HeuristicBoard mainBoard = new HeuristicBoard();
                mainBoard.SetBoard();
                UInt64 steps = 1;

                //main expanding loop
                while (!mainBoard.Done && !testForImpossible && !timeout)
                {
                    if (mainBoard.Method == 1)
                    {
                        testForImpossible = mainBoard.Astar_expand();
                    }
                    else
                    {
                        testForImpossible = mainBoard.BnB_expand();
                    }
                    List<Word> partialPaths = mainBoard.partialPaths;

                    //!(partialPaths.Equals(checkForUnchangedPaths)))// ||
                    //the impossible if statement 
                    if (((mainBoard.InitialWord.Count(x => x == '-')) != (mainBoard.InitialWord.Length / 2)) ||
                        ((mainBoard.GoalWord.Count(x => x == '-')) != (mainBoard.GoalWord.Length / 2)) ||
                        (mainBoard.InitialWord.Count(x => x == '-') == 0) ||
                        (mainBoard.InitialWord.Count(x => x != '-') == 0) ||
                        (mainBoard.GoalWord.Count(x => x == '-') == 0) ||
                        (mainBoard.GoalWord.Count(x => x != '-') == 0) ||
                        (mainBoard.InitialWord.Length < 3) ||
                        (mainBoard.GoalWord.Length < 3) || testForImpossible == true)
                        {
                            Console.WriteLine("Impossible / not within requriements.\n");
                            testForImpossible = true;
                        }
                    else if (steps == 4000000) //Times out after 4 million expands
                    {
                        timeout = true;
                    }
                    steps++;
                }
                if (!testForImpossible && !timeout)
                {
                    Console.WriteLine($"{mainBoard.GoalWord} found on step {steps}! \n");
                }
                if (timeout)
                {
                    Console.WriteLine("timeout. \n");
                }
            }
        }
    }
}
