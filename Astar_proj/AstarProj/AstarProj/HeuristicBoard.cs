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

        public void SetBoard()
        {
            Done = false;
            Console.WriteLine("Enter initial word: ");
            this.InitialWord = Console.ReadLine();

            Console.WriteLine("Enter goal word: ");
            this.GoalWord = Console.ReadLine();

            Word rootWord = new Word(InitialWord);
            partialPaths.Add(rootWord);
            GetChildren_fromJumps(rootWord);
        }

        List<Word> GetChildren_fromShifts(Word inputWord)
        {
            List<Word> shiftedPartialPaths = new List<Word>();
            string word = inputWord.CurrentState;
            int traveled = inputWord.DistanceTraveled;
            int toGo = inputWord.DistanceToGoal;
            string pastSteps = inputWord.PastSteps;

            string tempWord;
            char tempChar;

            for (int i = 0; i < word.Length; i++)
            {
                tempWord = word;
                // if first character
                if (i == 0 && word[i] != '-' && word[i + 1] == '-')
                {
                    tempChar = tempWord.ElementAt(i);
                    tempWord = tempWord.Remove(0, 1);
                    tempWord = tempWord.Insert(1, tempChar.ToString());

                    Word childPath = new Word(tempWord, traveled, pastSteps);
                    childPath.DistanceTraveled = childPath.DistanceTraveled + 1; //Shifts get distance increase by 1
                    shiftedPartialPaths.Add(childPath);
                }

                /*//if last character
                else if (i == word.Length - 1 && word[i] != '-' && word[i - 1] == '-')
                {
                    tempChar = tempWord.ElementAt(i);
                    tempWord = tempWord.Remove(tempWord.Length - 1);
                    tempWord = tempWord.Insert(tempWord.Length - 1, tempChar.ToString());

                    Word childPath = new Word(tempWord); //gotta be concerned about other parameters
                    shiftedPartialPaths.Add(childPath);
                }*/

                else if (i != 0 && i != word.Length - 1)
                {
                    if (word[i] != '-' && word[i + 1] == '-')
                    {
                        tempChar = tempWord.ElementAt(i);
                        tempWord = tempWord.Remove(i, 1); //used to be i!!!!
                        tempWord = tempWord.Insert(i + 1, tempChar.ToString());

                        Word childPath = new Word(tempWord, traveled, pastSteps);
                        childPath.DistanceTraveled = childPath.DistanceTraveled + 1; //Shifts get distance increase by 1
                        shiftedPartialPaths.Add(childPath);
                        tempWord = word;
                    }
                    
                    /*if (word[i] != '-' && word[i - 1] == '-')
                    {
                        tempChar = tempWord.ElementAt(i);
                        tempWord = tempWord.Remove(i, 1); //used to be i!!!!
                        tempWord = tempWord.Insert(i - 1, tempChar.ToString());

                        Word childPath = new Word(tempWord); //gotta be concerned about other parameters
                        shiftedPartialPaths.Add(childPath);
                    }*/
                }
            }

            return shiftedPartialPaths;
        }

        List<Word> GetChildren_fromJumps(Word inputWord)
        {
            List<Word> jumpedPartialPaths = new List<Word>();
            string word = inputWord.CurrentState;
            int traveled = inputWord.DistanceTraveled;
            int toGo = inputWord.DistanceToGoal;
            string pastSteps = inputWord.PastSteps;

            string tempWord;
            char tempChar;

            for (int i = 0; i < word.Length; i++)
            {
                tempWord = word;
                // if first character OR second
                if (i <= 1 && word[i] != '-' && word[i + 1] != '-' && word[i + 2] == '-')
                {
                    tempChar = tempWord.ElementAt(i);
                    tempWord = tempWord.Remove(i, 1);
                    tempWord = tempWord.Insert(i, "-");

                    tempWord = tempWord.Remove(i + 2, 1);
                    tempWord = tempWord.Insert(i + 2, tempChar.ToString());

                    Word childPath = new Word(tempWord, traveled, pastSteps);
                    childPath.DistanceTraveled = childPath.DistanceTraveled + 1; //Jumps get distance traveled values of ALSO 1
                    jumpedPartialPaths.Add(childPath);
                }

                /*//if last character
                else if (i >= word.Length - 2 && word[i] != '-' && word[i - 1] != '-' && word[i - 2] == '-')
                {
                    tempChar = tempWord.ElementAt(i);
                    tempWord = tempWord.Remove(tempWord.Length - 1);
                    tempWord = tempWord.Insert(tempWord.Length, "-");

                    tempWord = tempWord.Remove(tempWord.Length - 3, tempWord.Length - 2);
                    tempWord = tempWord.Insert(tempWord.Length - 2, tempChar.ToString());

                    Word childPath = new Word(tempWord); //gotta be concerned about other parameters
                    jumpedPartialPaths.Add(childPath);
                }*/

                else if (i > 1 &&  i < word.Length - 2)
                {
                    if (word[i] != '-' && word[i + 1] != '-' && word[i + 2] == '-')
                    {
                        tempChar = tempWord.ElementAt(i);
                        tempWord = tempWord.Remove(i, 1); 
                        tempWord = tempWord.Insert(i, "-");

                        tempWord = tempWord.Remove(i + 2, 1);
                        tempWord = tempWord.Insert(i + 2, tempChar.ToString());

                        Word childPath = new Word(tempWord, traveled, pastSteps);
                        childPath.DistanceTraveled = childPath.DistanceTraveled + 1; //Jumps get distance traveled values of ALSO 1
                        jumpedPartialPaths.Add(childPath);
                        tempWord = word;
                    }

                    /*if (word[i] != '-' && word[i - 1] != '-' && word[i - 2] == '-')
                    {
                        tempChar = tempWord.ElementAt(i);
                        tempWord = tempWord.Remove(i, 1);
                        tempWord = tempWord.Insert(i, "-");

                        tempWord = tempWord.Remove(i - 2, 1);
                        tempWord = tempWord.Insert(i - 2, tempChar.ToString());

                        Word childPath = new Word(tempWord); //gotta be concerned about other parameters
                        jumpedPartialPaths.Add(childPath);
                    }*/
                }
            }
            return jumpedPartialPaths;
        }

        List<Word> SortByDistanceTraveled (List <Word> unsortedList)
        {
            List<Word> sortedList = unsortedList.OrderBy(p => p.DistanceTraveled)/*.Reverse()*/.ToList();
            return sortedList;
        }

        public void BnB_expand()
        {
            List<Word> tempPaths = new List<Word>();

            //assemble childPaths to add to the front of the queue
            Word evaluation = partialPaths[0];
            List<Word> childShiftPaths = GetChildren_fromShifts(evaluation);
            List<Word> childJumpPaths = GetChildren_fromJumps(evaluation);
            List<Word> allChildrenPaths = new List<Word>(childShiftPaths.Concat(childJumpPaths));

            //memory managing for childPaths front of queue addition
            partialPaths.RemoveRange(0, 1);
            partialPaths.AddRange(allChildrenPaths);
            /*partialPaths.Clear();
            partialPaths = allChildrenPaths;
            partialPaths.AddRange(tempPaths);
            tempPaths.Clear();*/

            partialPaths = SortByDistanceTraveled(partialPaths);
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
            //for (int i = 0; i < paths.Count; i++)
            //{
            //Console.Write($"{paths[i].CurrentState} "); //Console.Write($"{paths[i].CurrentState} , {partialPaths[i].DistanceTraveled}\n");
            //}
            //Console.WriteLine("\n-----------------------------------------------------------------------------------------------------------");
            //Console.WriteLine(paths[0].CurrentState);
        }
    }
}
