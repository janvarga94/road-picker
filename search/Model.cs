using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RoadPicker.search
{
    public class Model
    {

        public static double stepMax = 40;
        public static double stepMultiplier = 0.5;      //0.5 is very good.. less than that will take longer time to find path
        public static double stepRadius = stepMax * stepMultiplier; //px asi
        public static State startState = null;
        public static State endState = null;
        public static Color roadColor; 
        public static List<Color> TolerantColors = new List<Color>(); // colors user added as road colors
        public static double angleIncrement = 0.2; //radians    -- prilikom pretrazivanja puteva, susedne tacke cu traziti rotiranjem za ovaj inkrement ugao
        public static double samePointAprox = stepRadius*0.4; //px       -- ako su tacke jako blizu, blize od ove vrednosti, onda cu ih posmatrati kao istu tacku
        public static double colorRoadTolerance = 20;       // this is tolerance for road color
        public static double colorSubTolerance = 20;     // tolerance for added color tolerances (street names, etc)
        public static double timeToSearch = 20;    //limit for search time, units are seconds

        private static System.Drawing.Bitmap _img;
        public static System.Drawing.Bitmap img {
            get
            {
                return _img;
            }
            set {
                _img = value;
                Model.pixels = new System.Drawing.Color[Model.img.Width, Model.img.Height];
      //          Console.WriteLine("pixels size: {0} {1} img size: {2} {3}",pixels.GetLength(0),pixels.GetLength(1),img.Width,img.Height);
                for (int i = 0; i < Model.img.Width; i++)
                {
                    for (int j = 0; j < Model.img.Height; j++)
                    {
                        Model.pixels[i,j] = Model.img.GetPixel(i,j);
                    }
                }
                ImageWidth = img.Width;
                ImageHeight = img.Height;
            }
        } // this image we dont use when searching solution, cuz concurrent problem, thats why we use pixels which are copy of this image
        public static System.Drawing.Color[,] pixels;
        public static double ImageWidth;
        public static double ImageHeight;
        public static double actualImageWidth;
        public static double actualImageHeight;
        public static List<LineCoords> wallLine;

        public static Boolean inTolerance(Color c1, Color c2, double colorTolerance) {
            if (c1.R - colorTolerance < c2.R &&
                c1.R + colorTolerance > c2.R &&
                c1.G - colorTolerance < c2.G &&
                c1.G + colorTolerance > c2.G &&
                c1.B - colorTolerance < c2.B &&
                c1.B + colorTolerance > c2.B )
            {
                return true;
            }
            return false;
        }
        public static Boolean inTolerance(jColor c1, jColor c2, double colorTolerance)
        {
            if (c1.R - colorTolerance < c2.R &&
                c1.R + colorTolerance > c2.R &&
                c1.G - colorTolerance < c2.G &&
                c1.G + colorTolerance > c2.G &&
                c1.B - colorTolerance < c2.B &&
                c1.B + colorTolerance > c2.B)
            {
                return true;
            }
            return false;
        }

        public static Boolean inTolerance(System.Drawing.Color c1, System.Drawing.Color c2, double colorTolerance) //this method had 0 references, could be removed
        {
            if (c1.R - colorTolerance < c2.R &&
                c1.R + colorTolerance > c2.R &&
                c1.G - colorTolerance < c2.G &&
                c1.G + colorTolerance > c2.G &&
                c1.B - colorTolerance < c2.B &&
                c1.B + colorTolerance > c2.B)
            {
                return true;
            }
            return false;
        }
        public static bool IsPathBetweenPointsClear(System.Windows.Point p1, System.Windows.Point p2, bool drawTrace = false)
        {
            double xStep = (p1.X - p2.X) / Model.stepRadius;
            double yStep = (p1.Y - p2.Y) / Model.stepRadius;
            double iX = p1.X, iY = p1.Y;
            //here we chack for colors
            for (int i = 0; i < Model.stepRadius; i++)
            {



                Color colorOfPoint = pointToColor(new System.Windows.Point(iX, iY));
                if (!Model.inTolerance(Model.roadColor, colorOfPoint, Model.colorRoadTolerance))
                {

                    //if we found any color in additional colors that user added, that matchech current collor, we wont retunr false
                       bool anyFound = TolerantColors.Any((color) => {
                           Color colorOfAnotherPoint = pointToColor(new System.Windows.Point(iX, iY));        
                           return inTolerance(color, colorOfAnotherPoint , colorSubTolerance);
                       });
                
                    if (!anyFound)
                    {
                      
                        return false;
                    }
                  


                   //    return false;
                }

                iX -= xStep;
                iY -= yStep;
            }
            //here we check for wall lines we drawn to canvasWall
            foreach (var line in wallLine)//.canvasWalls.Children)
            {
                //this guy represents function that connects p1 and p2
                double k1 = (p2.Y - p1.Y) / (p2.X - p1.X);
                double n1 = -p1.X * k1 + p1.Y;

                //this one is for each line
                double k2 = (line.Y2 - line.Y1) / (line.X2 - line.X1);
                double n2 = -line.X1 * k2 + line.Y1;

                if (k1.Equals(k2)) //that are paralel so its fine
                    continue;

                //intersection of linear functions
                double x = -(n1 - n2) / (k1 - k2);
                double y = k1 * x + n1;

                //now check if x,y are inside square defined by p1 and p2
                double minX = (line.X1 < line.X2 ? line.X1 : line.X2);
                double maxX = (line.X1 > line.X2 ? line.X1 : line.X2);

                double minY = (line.Y1 < line.Y2 ? line.Y1 : line.Y2);
                double maxY = (line.Y1 > line.Y2 ? line.Y1 : line.Y2);

                double y1 = k2 * p1.X + n2;
                double y2 = k2 * p2.X + n2;
                //if p1,p2 are from different side of wall, and if intersection is on wall, we return false
                if (x > minX &&
                    x < maxX &&
                    y > minY &&
                    y < maxY &&
                    ((p1.Y - y1 > 0 && p2.Y - y2 < 0) || (p1.Y - y1 < 0 && p2.Y - y2 > 0))
                    )
                {
                    //          MainWindow.instance.drawDot(new System.Windows.Point(x, y), 5, System.Windows.Media.Brushes.Black);
                    return false;  //this means wall is in the way
                }
                else
                {
                    //            MainWindow.instance.drawDot(new System.Windows.Point(x + 5, y + 5), 5, System.Windows.Media.Brushes.Red);
                }
            }

            return true;
        }
        public static System.Windows.Media.Color pointToColor(System.Windows.Point p)
        {
            double ratioX = p.X / actualImageWidth;
            double ratioY = p.Y / actualImageHeight;

            //     Console.WriteLine("Ratio : " + ratioX + " " + ratioY);
       
            int actualX = (int)(ratioX * ImageWidth);
            int actualY = (int)(ratioY * ImageHeight);

            return converters.Converter.colorSwitch(pixels[actualX, actualY]);
            

        }

        public static void prepareForSearch() {                  
            Model.wallLine = new List<Model.LineCoords>();
            foreach (var line in MainWindow.instance.canvasWalls.Children)
            {
                var lline = (Line)line;
                Model.wallLine.Add(new Model.LineCoords { X1 = lline.X1, X2 = lline.X2, Y1 = lline.Y1, Y2 = lline.Y2 });
            }
            stepRadius = stepMax * stepMultiplier;
            samePointAprox = stepRadius * 0.4;
        }





        public class LineCoords
        {
            public double X1, X2, Y1, Y2;
        }
    }
}
