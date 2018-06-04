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
            while (true)
            {
                bool impossible = false;
                HeuristicBoard mainBoard = new HeuristicBoard();
                mainBoard.SetBoard();
                UInt64 steps = 1;
                while (!mainBoard.Done && !impossible)
                {
                    //this temp data structure is used to determine if any moves are possible, and if not, killing the search.
                    List<Word> checkForUnchangedPaths = mainBoard.partialPaths; 
                    if (mainBoard.Method == 1)
                    {
                        mainBoard.Astar_expand();
                    }
                    else
                    {
                        mainBoard.BnB_expand();
                    }
                    List<Word> partialPaths = mainBoard.partialPaths;

                    if (((mainBoard.InitialWord.Count(x => x=='-')) != (mainBoard.InitialWord.Length/2)) ||
                        ((mainBoard.GoalWord.Count(x => x == '-')) != (mainBoard.GoalWord.Length / 2))   ||
                        !(partialPaths.Equals(checkForUnchangedPaths))                                   ||
                        (mainBoard.InitialWord.Count(x => x == '-') == 0)                                ||
                        (mainBoard.InitialWord.Count(x => x != '-') == 0)                                ||
                        (mainBoard.GoalWord.Count(x => x == '-') == 0)                                   ||
                        (mainBoard.GoalWord.Count(x => x != '-') == 0)                                   ||
                        (mainBoard.InitialWord.Length > 2)                                               ||
                        (mainBoard.GoalWord.Length > 2))
                    {
                        Console.WriteLine("Impossible / not within requriements.\n");
                        impossible = true;
                    }
                    steps++;
                }
                if (!impossible)
                {
                    Console.WriteLine($"{mainBoard.GoalWord} found on step {steps}! \n");
                }
            }
        }
    }
}
