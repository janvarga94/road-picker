using RoadPicker.search;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media;

namespace RoadPicker
{
    public class NeuralSomething
    {
        private int _cols;
        private int _rows;

        private int _pixelSamples;

        private jColor[,] _matrix;

        public jColor[,] Matrix { get { return _matrix; } set { _matrix = value; } }

        private List<jColor> _lostOfChoosen = new List<jColor>();

        private double _propagaionMaxValue = 5;

        private Random _rnd = new Random();
        public NeuralSomething(int cols, int rows, int pixelSamples)
        {
            _cols = cols;
            _rows = rows;
            _pixelSamples = pixelSamples;
            _matrix = new jColor[_cols, _rows];

            randomizeValues();
        }
        private void randomizeValues()
        {
            var rnd = new Random();

            for (int i = 0; i < _cols; i++)
            {
                for (int j = 0; j < _rows; j++)
                {
                    _matrix[i, j] = new jColor(rnd.Next(0, 256), rnd.Next(0, 256), rnd.Next(0, 256));
                }
            }
        }
        public Bitmap DoWhatUrMadeFor(Bitmap bmp)
        {
            double ratio = bmp.Height / (double)bmp.Width;


            var newWidth = Math.Sqrt(_pixelSamples / ratio);
            var newHeight = newWidth * ratio;


            var stepX = bmp.Width / newWidth;
            var stepY = bmp.Height / newHeight;

             //OK:     Console.WriteLine("newW {0}, newH {1}", newWidth, newHeight);
            //OK: Console.WriteLine("steps: {0} {1}",stepX,stepY);
            var maxIterations = 100;

            for (int i = 0; i < newWidth; i++)
            {
                for (int j = 0; j < newHeight; j++)
                {
                    var pixel = bmp.GetPixel((int)(i * stepX), (int)(j * stepY));
                    //OK seems:      Console.WriteLine("pixel coords: {0} {1}", (int)(i * stepX), (int)(j * stepY));
                    var jcolor = new jColor(pixel.R, pixel.G, pixel.B);

                    if (!_lostOfChoosen.Any(p => Model.inTolerance(p, jcolor, 5)))
                        _lostOfChoosen.Add(jcolor);
                }
            }

            for (int iter = 0; iter < maxIterations; iter++)
            {
          
                foreach (var jcolor in _lostOfChoosen) {
                    Point winnerPoint = findWinner(jcolor);
                    //           Console.WriteLine("winner coords: " + winnerPoint.X + " " + winnerPoint.Y);
                    var winner = _matrix[winnerPoint.X, winnerPoint.Y];

                    fixNeuronToBeMoreLikePixel(winnerPoint.X, winnerPoint.Y, jcolor, _propagaionMaxValue);

                    var colorOfWinner = new jColor(winner.R, winner.G, winner.B);
                    //here we set either color of neuron, or choosen pixel
                    fixAroundNeuron(winnerPoint.X, winnerPoint.Y, _propagaionMaxValue * 0.5, 1, jcolor);
                    fixAroundNeuron(winnerPoint.X, winnerPoint.Y, _propagaionMaxValue * 0.25, 2, jcolor);
                    fixAroundNeuron(winnerPoint.X, winnerPoint.Y, _propagaionMaxValue * 0.125, 3, jcolor);
                    //    fixAroundNeuron(winnerPoint.X, winnerPoint.Y, _propagaionMaxValue * 0.1, 4, colorOfWinner);
                }
            }           
            return null;
        }

        public Point findWinner(jColor c)
        {
            List<Point> bestNeurons = new List<Point>();
            bestNeurons.Add(new Point(0, 0));
            double closestDistance = Double.MaxValue;
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    var tempColor = _matrix[i, j];
                    //distance of colors
                    var distance = Math.Sqrt(Math.Pow((c.R - tempColor.R), 2) + Math.Pow((c.G - tempColor.G), 2) + Math.Pow((c.B - tempColor.B), 2));

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        bestNeurons = new List<Point>();
                        bestNeurons.Add(new Point(i, j));
                    }
                    else if (distance.Equals(closestDistance)) {
                        bestNeurons.Add(new Point(i, j));
                        //    Console.WriteLine("equals");
                    }
                }
            }

            var random = _rnd.Next(0, bestNeurons.Count);
            return bestNeurons[random];
        }

        //return error
        private void fixNeuronToBeMoreLikePixel(int x, int y, jColor pixel, double fix_max_distance)
        {
            if (x < 0 || y < 0 || x >= _cols || y >= _rows)
                return;

            var choosen = _matrix[x, y];
            //     Console.WriteLine("winner before: " + choosen.R);
            var fix_R = choosen.R - pixel.R;
            var fix_G = choosen.G - pixel.G;
            var fix_B = choosen.B - pixel.B;
            //      Console.WriteLine("pixel we fit to" + pixel.R);
            fix_R = InBounds(fix_R, fix_max_distance);
            fix_G = InBounds(fix_G, fix_max_distance);
            fix_B = InBounds(fix_B, fix_max_distance);

            //OK seems       Console.WriteLine("fixing {0} {1}, before: " + _matrix[x, y].R + " " + _matrix[x, y].G + " " + _matrix[x, y].B, x,y);
            choosen.R -= fix_R;
            choosen.G -= fix_G;
            choosen.B -= fix_B;

            var boundNeuron = InColorBounds(choosen);

            _matrix[x, y].R = boundNeuron.R;
            _matrix[x, y].G = boundNeuron.G;
            _matrix[x, y].B = boundNeuron.B;

            //     Console.WriteLine("winner after: " + choosen.R);
        }
        /*      private void fixToBeMoreLike(jColor referent, int x, int y, double fix_max_distance)
              {
                  if (x >= 0 && x < _cols && y >= 0 && y < _rows)
                  {
                      var neuron = _matrix[x, y];
                      var R_distance = referent.R - neuron.R;
                      var G_distance = referent.G - neuron.B;
                      var B_distance = referent.B - neuron.B;

                      R_distance = InBounds(R_distance, fix_max_distance);
                      G_distance = InBounds(G_distance, fix_max_distance);
                      B_distance = InBounds(B_distance, fix_max_distance);

                      neuron.R += R_distance;
                      neuron.G += G_distance;
                      neuron.B += B_distance;

                      var boundNeuron = InColorBounds(neuron);

                      _matrix[x, y].R = boundNeuron.R;
                      _matrix[x, y].G = boundNeuron.G;
                      _matrix[x, y].B = boundNeuron.B;               
                  }
              }*/
        private void fixAroundNeuron(int x, int y, double fix_max_distance, int neuronDistance, jColor pixel) { //x,y are coords of choosen

            var bottomTopCount = neuronDistance * 2 + 1;
            //      Console.WriteLine("fixing around width distance: " + neuronDistance);
            //left side
            for (int i = 0; i < bottomTopCount; i++) {
                fixNeuronToBeMoreLikePixel(x - neuronDistance, y - neuronDistance + i, pixel, fix_max_distance);
                //          Console.WriteLine("x , y: {0} {1}", x - neuronDistance, y - neuronDistance + i);
            }
            //right side
            for (int i = 0; i < bottomTopCount; i++)
            {
                fixNeuronToBeMoreLikePixel(x + neuronDistance, y - neuronDistance + i, pixel, fix_max_distance);
                //            Console.WriteLine("x , y: {0} {1}", x + neuronDistance, y - neuronDistance + i);

            }

            var topDownCount = neuronDistance * 2 - 1;

            //top side
            for (int i = 0; i < topDownCount; i++)
            {
                fixNeuronToBeMoreLikePixel(x + i - neuronDistance + 1, y - neuronDistance, pixel, fix_max_distance);
                //           Console.WriteLine("x , y: {0} {1}", x + i - neuronDistance + 1, y - neuronDistance);

            }
            //bottom side
            for (int i = 0; i < topDownCount; i++)
            {
                fixNeuronToBeMoreLikePixel(x + i - neuronDistance + 1, y + neuronDistance, pixel, fix_max_distance);
                //        Console.WriteLine("x , y: {0} {1}", x + i - neuronDistance + 1, y + neuronDistance);
            }
            //      Console.WriteLine("/fixing around");
        }
        private double InBounds(double value, double bounds)
        {
            if (Math.Abs(value) > bounds)
            {
                if (value < 0)
                {
                    value = -bounds;
                }
                else
                    value = bounds;
            }

            return value;
        }
        private jColor InColorBounds(jColor color)
        {
            if (color.R < 0) color.R = 0;
            if (color.R > 255) color.R = 255;

            if (color.G < 0) color.G = 0;
            if (color.G > 255) color.G = 255;

            if (color.B < 0) color.B = 0;
            if (color.B > 255) color.B = 255;

            return color;
        }
        private void printNetwork()
        {
            Console.WriteLine("NETWORK: ------------------");
            for (int i = 0; i < _cols; i++)
            {
                for (int j = 0; j < _rows; j++)
                {
                    var neuron = _matrix[i, j];
                    Console.WriteLine("neuron {3} {4} : {0} {1} {2}", neuron.R, neuron.G, neuron.B, i, j);
                }
            }
            Console.WriteLine("/NETWORK: ------------------");
        }
        public jColor getColorAt(int x, int y){

                       
                return _matrix[x, y];
           
        }

    }

    public struct jColor
    {
        public double R, G, B;
        public jColor(double r, double g, double b)
        {
            R = r;
            G = g;
            B = b;
        }
    }
}
