using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstarProj
{
    public class HeuristicBoard
    {
        public string InitialWord { get; set; }
        public string GoalWord { get; set; }
        public List<Word> partialPaths = new List<Word>();
        public bool Done { get; set; }
        public int Method { get; set; }

        public void SetBoard()
        {
            Done = false;
            Console.WriteLine("Enter initial word: (Type EXIT to end)");
            this.InitialWord = Console.ReadLine();
            if(this.InitialWord == "EXIT")
            {
                Environment.Exit(0);
            }

            Console.WriteLine("Enter goal word: ");
            this.GoalWord = Console.ReadLine();

            Console.WriteLine("Enter 0 for Branch and Bound and Enter 1 for A*");
            this.Method = Int32.Parse(Console.ReadLine());

            Word rootWord = new Word(InitialWord);
            partialPaths.Add(rootWord);
        }

        List<Word> GetChildren_fromShifts(Word inputWord, int Mode) //Mode: 0 == BnB, 1 == Astar
        {
            List<Word> shiftedPartialPaths = new List<Word>();
            string word = inputWord.CurrentState;
            int traveled = inputWord.DistanceTraveled;
            int toGo = inputWord.DistanceToGoal;
            string pastSteps = inputWord.PastSteps;

            string tempWord;
            char tempChar;
            bool moveMade = false;

            for (int i = 0; i < word.Length; i++)
            {
                tempWord = word;
                moveMade = false;
                // if first character
                if (i == 0 && word[i] != '-' && word[i + 1] == '-')
                {
                    tempChar = tempWord.ElementAt(i);
                    tempWord = tempWord.Remove(0, 1);
                    tempWord = tempWord.Insert(1, tempChar.ToString());
                    moveMade = true;
                }

                else if (i != 0 && i != word.Length - 1)
                {
                    if (word[i] != '-' && word[i + 1] == '-')
                    {
                        tempChar = tempWord.ElementAt(i);
                        tempWord = tempWord.Remove(i, 1); //used to be i!!!!
                        tempWord = tempWord.Insert(i + 1, tempChar.ToString());
                        moveMade = true;
                    }
                }
                if (Mode == 0 && moveMade)
                {
                    Word childPath = new Word(tempWord, traveled, pastSteps);
                    childPath.DistanceTraveled = childPath.DistanceTraveled + 1; //Shifts get distance increase by 1
                    shiftedPartialPaths.Add(childPath);
                }
                else if (Mode == 1 && moveMade)
                {
                    int estimator = getEstimator(tempWord);
                    Word childPath = new Word(tempWord, traveled, toGo, pastSteps);
                    childPath.DistanceTraveled = childPath.DistanceTraveled + 1; //Shifts get distance increase by 1
                    childPath.DistanceToGoal = childPath.DistanceTraveled + estimator;
                    shiftedPartialPaths.Add(childPath);
                }
            }
            return shiftedPartialPaths;
        }

        List<Word> GetChildren_fromJumps(Word inputWord, int Mode)
        {
            List<Word> jumpedPartialPaths = new List<Word>();
            string word = inputWord.CurrentState;
            int traveled = inputWord.DistanceTraveled;
            int toGo = inputWord.DistanceToGoal;
            string pastSteps = inputWord.PastSteps;

            string tempWord;
            char tempChar;
            bool moveMade = false;

            for (int i = 0; i < word.Length; i++)
            {
                tempWord = word;
                moveMade = false;
                // if first character OR second
                if (i <= 1 && word[i] != '-' && word[i + 1] != '-' && word[i + 2] == '-')
                {
                    tempChar = tempWord.ElementAt(i);
                    tempWord = tempWord.Remove(i, 1);
                    tempWord = tempWord.Insert(i, "-");

                    tempWord = tempWord.Remove(i + 2, 1);
                    tempWord = tempWord.Insert(i + 2, tempChar.ToString());
                    moveMade = true;
                }

                else if (i > 1 &&  i < word.Length - 2)
                {
                    if (word[i] != '-' && word[i + 1] != '-' && word[i + 2] == '-')
                    {
                        tempChar = tempWord.ElementAt(i);
                        tempWord = tempWord.Remove(i, 1); 
                        tempWord = tempWord.Insert(i, "-");

                        tempWord = tempWord.Remove(i + 2, 1);
                        tempWord = tempWord.Insert(i + 2, tempChar.ToString());
                        moveMade = true;
                    }
                }

                if (Mode == 0 && moveMade)
                {
                    Word childPath = new Word(tempWord, traveled, pastSteps);
                    childPath.DistanceTraveled = childPath.DistanceTraveled + 1; //Jumps get distance traveled values of ALSO 1
                    jumpedPartialPaths.Add(childPath);
                }
                else if (Mode == 1 && moveMade)
                {
                    int estimator = getEstimator(tempWord);
                    Word childPath = new Word(tempWord, traveled, toGo, pastSteps);
                    childPath.DistanceTraveled = childPath.DistanceTraveled + 1; //Jumps get distance traveled values of ALSO 1
                    childPath.DistanceToGoal = childPath.DistanceTraveled + estimator;
                    jumpedPartialPaths.Add(childPath);
                }
            }
            return jumpedPartialPaths;
        }

        List<Word> Sort_BnB (List <Word> unsortedList)
        {
            List<Word> sortedList = unsortedList.OrderByDescending(p => p.DistanceTraveled).ToList();
            return sortedList;
        }

        List<Word> Sort_Astar(List<Word> unsortedList)
        {
            List<Word> sortedList = unsortedList.OrderByDescending(p => p.DistanceToGoal).ToList();
            return sortedList;
        }

        public void BnB_expand()
        {
            List<Word> tempPaths = new List<Word>();

            //assemble childPaths to add to the front of the queue
            Word evaluation = partialPaths[0];
            List<Word> childShiftPaths = GetChildren_fromShifts(evaluation, 0);
            List<Word> childJumpPaths = GetChildren_fromJumps(evaluation, 0);
            List<Word> allChildrenPaths = new List<Word>(childShiftPaths.Concat(childJumpPaths));

            //memory managing for childPaths front of queue addition
            partialPaths.RemoveRange(0, 1);
            partialPaths.AddRange(allChildrenPaths);

            partialPaths = Sort_BnB(partialPaths);
            CheckIfGoalStateFound(partialPaths);
        }

        public int getEstimator(string inputWord)
        {
            int charactersMatched = 0;

            for (int i = 0; i < inputWord.Length; i++)
            {
                if (inputWord[i] == GoalWord[i])
                {
                    charactersMatched = charactersMatched + 1;
                }
            }
            return charactersMatched * 2;
        }

        List<Word> RemoveRedundantPaths(List<Word> inputPaths)
        {
            List<string> wordBank = new List<string>();
            for (int i = 0; i < inputPaths.Count; i++)
            {
               if (wordBank.Contains(inputPaths[i].CurrentState))
               {
                    inputPaths.Remove(inputPaths[i]);
               }
               else
               {
                    wordBank.Add(inputPaths[i].CurrentState);
               }
            }
            return inputPaths;
        }

        public void Astar_expand()
        {
            List<Word> tempPaths = new List<Word>();

            //assemble childPaths to add to the front of the queue
            Word evaluation = partialPaths[0];
            List<Word> childShiftPaths = GetChildren_fromShifts(evaluation, 1);
            List<Word> childJumpPaths = GetChildren_fromJumps(evaluation, 1);
            List<Word> allChildrenPaths = new List<Word>(childShiftPaths.Concat(childJumpPaths));

            //memory managing for childPaths front of queue addition
            partialPaths.RemoveRange(0, 1);
            partialPaths.AddRange(allChildrenPaths);

            partialPaths = RemoveRedundantPaths(partialPaths);
            partialPaths = Sort_Astar(partialPaths);
            CheckIfGoalStateFound(partialPaths);
        }

        public void CheckIfGoalStateFound(List<Word> paths)
        {
            if (paths[0].CurrentState == GoalWord)
            {
                Done = true;
                Console.WriteLine(paths[0].PastSteps);
            }
        }

        public void PrintPartialPaths(List<Word> paths)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                Console.Write($"{paths[i].CurrentState} "); //Console.Write($"{paths[i].CurrentState} , {partialPaths[i].DistanceTraveled}\n");
            }
            Console.WriteLine("\n-----------------------------------------------------------------------------------------------------------");
        }
    }
}
