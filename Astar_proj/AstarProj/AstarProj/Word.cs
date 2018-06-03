using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstarProj
{
    public class Word
    {
        public string CurrentState { get; set; }
        public int DistanceTraveled { get; set; }
        public int DistanceToGoal { get; set; }
        public string PastSteps { get; set; }

        public Word(string initialState)
        {
            this.CurrentState = initialState;
            this.DistanceTraveled = 0;
            this.DistanceToGoal = 0;
        }

        public Word(string initialState, int distance, string previousSteps)
        {
            this.CurrentState = initialState;
            this.DistanceTraveled = distance;
            this.DistanceToGoal = -1;
            this.PastSteps = previousSteps + initialState + " ";

        }
        public Word(string initialState, int distance, int toGo, string previousSteps)
        {
            this.CurrentState = initialState;
            this.DistanceTraveled = distance;
            this.DistanceToGoal = toGo;
            this.PastSteps = previousSteps + initialState + " ";
        }
    }

   
}
