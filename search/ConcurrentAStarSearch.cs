using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadPicker.search
{
    class ConcurrentAStarSearch
    {
        public List<State> lastPositions = new List<State>();
        List<State> onTreatment = new List<State>();
        private DateTime startTime;
        public State search(State startState, DoWorkEventArgs e, BackgroundWorker bw)
        {
            startTime = DateTime.Now;
            onTreatment.Add(startState);

            while (onTreatment.Count > 0)
            {
                if (bw.CancellationPending) return null;

                if ((DateTime.Now - startTime).TotalSeconds > Model.timeToSearch) { return null; }
                //we will create maximun 8 threads
                int threadNum = onTreatment.Count >= 8 ? 8 : onTreatment.Count; // 8 seemed optimal for my pc
                //now we get best states
                State[] best = new State[threadNum];
                
                for (int i = 0; i < threadNum; i++) {
                    State bestState = getBest();
                    best[i] = bestState;
                    onTreatment.Remove(bestState);         
                }

                //each thread will return child states that can be added to search list (onTreatment)
                Task<List<State>>[] threads = new Task<List<State>>[threadNum];

                for (int i = 0; i < threadNum; i++)
                {
                    int saveI = i; //we have to go this way, because i might increment during thread procces, but we want good old i 
                    threads[saveI] = Task<List<State>>.Factory.StartNew(() =>
                    {  
                        List<State> childernWeWillConsider = new List<State>();
                        List<State> possiableNextStates = best[saveI].possiableNextStates();
                        foreach (State nextState in possiableNextStates)
                        {
                            //if nextState is last state
                            if (nextState.AproxSame(Model.endState))
                            {
                                childernWeWillConsider.Add(nextState);                              
                                break;
                            }
                            else   //if we already been there                                                  
                            if (!lastPositions.Any(s => s.AproxSame(nextState)))
                            {
                                childernWeWillConsider.Add(nextState);
                            }
                        }
                        return childernWeWillConsider;
                    });
                }

                //we wait while thay finish job
                Task.WaitAll(threads);

                //collect results
                List<State>[] results = new List<State>[threadNum];
 
                for (int i = 0; i < threadNum; i++)
                {
                    results[i] =  threads[i].Result;
                }

                List<State> merged = new List<State>();
                foreach (var res in results)
                {
                    foreach (var state in res)
                    {
                        if (!merged.Any(s => s.AproxSame(state))) {
                            merged.Add(state);
                        }
                    }
                }
               //test if merged have effect, it doest, up to 10 times smaller than all togater results
         /*        Console.WriteLine("Merged size: {0}, not merged size {1}", merged.Count, results.Aggregate((a, b) =>
                {
                    a.AddRange(b);
                    return a;
                }
                ).Count);*/
  
                foreach (var state in merged)
                {
                //    Console.WriteLine("path length " + state.path().Count);
                    if (state.AproxSame(Model.endState)) {                         
                            return state;
                    }
                        
                    if (!lastPositions.Any(s => s.AproxSame(state)))
                    {
                        lastPositions.Add(state);
                        onTreatment.Add(state);
                    }
                }
                
           }
            return null;
        }

        public State getBest()
        {
            double min = double.MaxValue;
            State bestState = onTreatment[0];
            foreach (State state in onTreatment)
            {
                double predict = state.distance + Math.Sqrt(Math.Pow(state.position.X - Model.endState.position.X, 2) + Math.Pow(state.position.Y - Model.endState.position.Y, 2));
                if (predict < min)
                {
                    min = predict;
                    bestState = state;
                }
            }
            return bestState;
        }
    }
}
