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
            HeuristicBoard mainBoard = new HeuristicBoard();
            mainBoard.SetBoard();
            Int64 i = 0;
            while (!mainBoard.Done/*i != 100*/)
            {
                mainBoard.BnB_expand();
                List<Word> partialPaths = mainBoard.partialPaths;
                mainBoard.PrintPartialPaths(partialPaths);
                i++;
            }
            Console.WriteLine($"{mainBoard.GoalWord} found on step {i + 1}! ");
            Console.ReadKey();
        }
    }
}
