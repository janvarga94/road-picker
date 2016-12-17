using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadPicker.search
{
    class AStarSearch
    {
        public List<State> lastPositions = new List<State>();
        List<State> onTreatment = new List<State>();
        private DateTime startTime;
        public State search(State startState)
        {
            startTime = DateTime.Now;
            onTreatment.Add(startState);

            while (onTreatment.Count > 0)
            {
                if ((DateTime.Now - startTime).TotalSeconds > Model.timeToSearch) { return null; }

                State treated = getBest(); //onTreatment[0];
                List<State> possiableNextStates = treated.possiableNextStates();
                foreach (State nextState in possiableNextStates)
                {
                    //if nextState is last state
                    if (nextState.AproxSame(Model.endState))
                    {            
                        return nextState;
                    }

                    //if we already been there
                    if (!lastPositions.Any(s => s.AproxSame(nextState)))
                    {
                        onTreatment.Add(nextState);
                        lastPositions.Add(nextState);
                    }
                }
                onTreatment.Remove(treated);
            }
            return null;
        }

        public State getBest() {
            double min = double.MaxValue;
            State bestState = onTreatment[0];
            foreach (State state in onTreatment) {
                double predict = state.distance + Math.Sqrt(Math.Pow(state.position.X - Model.endState.position.X,2) + Math.Pow(state.position.Y - Model.endState.position.Y, 2));
                if (predict < min) {
                    min = predict;
                    bestState = state;
                }
            }
            return bestState;
        }
    }
}
