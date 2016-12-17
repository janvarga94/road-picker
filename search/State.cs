using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RoadPicker.search
{
    public class State
    {

        public State parent;
        public System.Windows.Point position;
        public double distance = 0;

        public List<State> possiableNextStates()
        {
            List<State> children = new List<State>();

            for (double angle = 0; angle < 2 * Math.PI; angle += Model.angleIncrement)
            {
                System.Windows.Point newbie = new System.Windows.Point();
                newbie.X = position.X + Math.Cos(angle) * Model.stepRadius;
                newbie.Y = position.Y + Math.Sin(angle) * Model.stepRadius;

                //now we check if point if legal
                if (newbie.X <= 0 || newbie.Y <= 0 || newbie.X >= Model.actualImageWidth || newbie.Y >= Model.actualImageHeight)
                    continue;

                if (!Model.IsPathBetweenPointsClear(position, newbie))
                {
                    continue;
                }

                State child = new State { position = newbie, parent = this, distance = this.distance + Model.stepRadius };
                children.Add(child);
            }
            return children;
        }


        public bool isKrajnjeStanje()
        {
            if (this.AproxSame(Model.endState))
                return true;
            else
                return false;
        }

        public List<State> path()
        {
            List<State> putanja = new List<State>();
            State tt = this;
            while (tt != null)
            {
                putanja.Insert(0, tt);
                tt = tt.parent;
            }
            return putanja;
        }
        public bool AproxSame(State state)  //ako ih mogu posmatrati kao istu tacku
        {
            double x1 = position.X;
            double y1 = position.Y;

            double x2 = state.position.X;
            double y2 = state.position.Y;

            double radius = Model.samePointAprox;

            if (x1 - radius < x2 &&
                 x1 + radius > x2 &&
                 y1 - radius < y2 &&
                 y1 + radius > y2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
