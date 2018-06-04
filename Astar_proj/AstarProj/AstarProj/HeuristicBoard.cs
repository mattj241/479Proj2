using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstarProj
{
    //Meat and potatoes class, all heuristics done here
    public class HeuristicBoard
    {
        public string InitialWord { get; set; } //start state word
        public string GoalWord { get; set; } //end state word
        public List<Word> partialPaths = new List<Word>(); //queue of partial paths
        public bool Done { get; set; } //flag for goal word found or not
        public int Method { get; set; } //Mode: 0 == BnB, 1 == Astar


        /*
         Description: Configures the board with basic input to get started finding paths, like start state, end state, and search mode
         Pre: none
         Post: The root/initial word is set in the partial paths queue
        */
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

        /*
        Description: Assembles 1 of 2 partial path queues with the children of the first parent node. This is move generator 1 of 2.
        The name explains what it does, it generates any moves for shifts to the right for any letter in the string. Ex: ART--- -> AR-T--
        Pre: The first Word in the queue is passed in and evaluated for moves, and it must know what mode (BnB or A*) we're working with too.
        Post: A list of moves that were generated are returned to the expand method
       */
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
                        tempWord = tempWord.Remove(i, 1);
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

        /*
        Description: Assembles 1 of 2 partial path queues with the children of the first parent node. This is move generator 2 of 2.
        It generates any moves for JUMPS to the right for any letter in the string. Ex: ART--- -> A-TR--
        Pre: The first Word in the queue is passed in and evaluated for moves, and it must know what mode (BnB or A*) we're working with too.
        Post: A list of moves that were generated are returned to the expand method
       */
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
                try
                {
                    if (i <= 1 && word[i] != '-' && word[i + 1] != '-' && word[i + 2] == '-')
                    {
                        tempChar = tempWord.ElementAt(i);
                        tempWord = tempWord.Remove(i, 1);
                        tempWord = tempWord.Insert(i, "-");

                        tempWord = tempWord.Remove(i + 2, 1);
                        tempWord = tempWord.Insert(i + 2, tempChar.ToString());
                        moveMade = true;
                    }

                    else if (i > 1 && i < word.Length - 2)
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
                }
                catch
                {
                    //DO nothing, exception handled by main
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

        /*
        Description: Sorts the queue of partial paths by distance traveled, a la Branch and Bound
        Pre: Passes in the queue of partial paths that just added the children from the last move generation
        Post: Sorted list to end the expanding round for BnB is returned back to the expand method
       */
        List<Word> Sort_BnB (List <Word> unsortedList)
        {
            List<Word> sortedList = unsortedList.OrderByDescending(p => p.DistanceTraveled).ToList();
            return sortedList;
        }

        /*
        Description: Sorts the queue of partial paths by Distance to goal (plus distance traveled), a la A*
        Pre: Passes in the queue of partial paths that just added the children from the last move generation
        Post: Sorted list to end the expanding round for A* is returned back to the expand method
       */
        List<Word> Sort_Astar(List<Word> unsortedList)
        {
            List<Word> sortedList = unsortedList.OrderByDescending(p => p.DistanceToGoal).ToList();
            return sortedList;
        }

        /*
        Description: this is the estimator for the A* algorithm. With input, it underestimates the distance to the goal by giving a value based on
        how close each letter is to where it needs to be in the goal state, so it essentially underestimates by underestimating the proximity to the goal state.
        Pre: Passes only the current string state of the word being evaluated by the expand method.
        Post: the estimator value is calculated and returned back to the expand method
       */
        public int getEstimator(string inputWord)
        {
            int estimatorValue = 0;
            int maxAwardedValue = inputWord.Length - 1;
            for (int i = 0; i < inputWord.Length; i++)
            {
                if (inputWord[i] != '-')
                {
                    int indexInGoalWord = GoalWord.IndexOf(inputWord[i]);
                    int indicesDelta = indexInGoalWord - i;
                    int dupCount = inputWord.Count(x => x == inputWord[i]);
                    if (indicesDelta < 0 && dupCount < 2) //if any letter is found past where it should be, the current path is discredited.
                    {
                        indicesDelta = 10000;
                    }
                    estimatorValue = estimatorValue + (maxAwardedValue - indicesDelta);
                }
            }
            return estimatorValue;
        }

        /*
        Description: A* crucial feature that does what its called. All it does is a quick run through of the partial paths queue and determines if its seen anything
        it already saw earlier when traversing the queue. If there is a duplicate item, its deleted. 
        Pre: Passes the queue of partial paths in for evaluation, expand method just finished the move generations
        Post: Hopefully a returned queue of partial paths is returned trimmed for faster overall evaluation.
       */
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

        /*
        Description: BnB expand method. Starts with move generation, followed by removal or first node, 
        then adding the move generation queue to the master partial paths queue. It finishes by sorting the finished queue and checking if its done.
        Pre: n/a
        Post: the expand step is completed and ready for the next step if needed
       */
        public bool BnB_expand()
        {
            List<Word> tempPaths = new List<Word>();
            try
            {
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
                return false;
            }
            catch
            {
                return true;
            }
        }

        /*
        Description: A* expand method. Starts with move generation, followed by removal or first node, 
        then adding the move generation queue to the master partial paths queue. It finishes by first removing redundant paths, 
        then sorting the finished queue and checking if its done.
        Pre: n/a
        Post: the expand step is completed and ready for the next step if needed
       */
        public bool Astar_expand()
        {
            List<Word> tempPaths = new List<Word>();
            try
            {
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
                return false;
            }
            catch
            {
                return true;
            }
        }

        /*
        Description: Simply checks if the first element in the partial paths queue is the goal state after the expansion has been done.
        Pre: Passes the queue of partial paths in for evaluation
        Post: If done, the past steps taken by the path are printed to the user and then the next iteration is available for the user to start.
       */
        public void CheckIfGoalStateFound(List<Word> paths)
        {
            try
            {
                if (paths[0].CurrentState == GoalWord)
                {
                    Done = true;
                    Console.WriteLine(paths[0].PastSteps);
                }
            }
            catch
            {
                //DO nothing, excpetion handled in main
            }
        }
    }
}
