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
                HeuristicBoard mainBoard = new HeuristicBoard();
                mainBoard.SetBoard();
                UInt64 steps = 1;
                while (!mainBoard.Done)
                {
                    if (mainBoard.Method == 1)
                    {
                        mainBoard.Astar_expand();
                    }
                    else
                    {
                        mainBoard.BnB_expand();
                    }
                    List<Word> partialPaths = mainBoard.partialPaths;
                    steps++;
                }
                Console.WriteLine($"{mainBoard.GoalWord} found on step {steps}! \n");
            }
            //Console.ReadKey();
        }
    }
}
