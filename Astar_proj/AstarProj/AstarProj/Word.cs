using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstarProj
{
    //Defining of the main data structure used in the program
    public class Word
    {
        public string CurrentState { get; set; } //this is the exact arrangment of the letters and dashes at a given point
        public int DistanceTraveled { get; set; } //Records distance traveled, the number of identifiable of "past steps" found in PastSteps below
        public int DistanceToGoal { get; set; } //Records distance to goal, which is the estimator for A*
        public string PastSteps { get; set; } //An often concatenated string of all the states of the main string leading up to the current point

        //1 of 3 constructors, this is basic Word initialization only used by the SetBoard method, or first step in the heuristic search
        public Word(string initialState)
        {
            this.CurrentState = initialState;
            this.DistanceTraveled = 0;
            this.DistanceToGoal = 0;
        }

        //2 of 3, this constructor is used to create the data type when expanding with branch and bound
        public Word(string initialState, int distance, string previousSteps)
        {
            this.CurrentState = initialState;
            this.DistanceTraveled = distance;
            this.DistanceToGoal = -1;
            this.PastSteps = previousSteps + initialState + " ";

        }

        //3 of 3, this constructor is used to create the data type when expanding with A*
        public Word(string initialState, int distance, int toGo, string previousSteps)
        {
            this.CurrentState = initialState;
            this.DistanceTraveled = distance;
            this.DistanceToGoal = toGo;
            this.PastSteps = previousSteps + initialState + " ";
        }
    }

   
}
