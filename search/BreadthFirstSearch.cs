using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoadPicker.search
{
    class BreadthFirstSearch
    {
        public List<State> lastPositions = new List<State>();

        //outdated
 /*       public State search(State startState)
        {
            List<State> onTreatment = new List<State>();
                        onTreatment.Add(startState);
           
            while (onTreatment.Count > 0)
            {
                State treated = onTreatment[0];
                List<State> possiableNextStates = treated.possiableNextStates();
                foreach (State nextState in possiableNextStates)
                {
                    //if nextState is last state
                    if (nextState.AproxSame(Model.endState)) {
                        Console.WriteLine("next: " + nextState.position + " end: " + Model.endState.position);
                        return nextState;
                    }

                    if (!lastPositions.Any(s => s.AproxSame(nextState)))
                    {         
                        onTreatment.Add(nextState);
                        lastPositions.Add(nextState);
                    }
         
                }
                onTreatment.Remove(treated);
            }
            return null;
        }*/
    }
}
