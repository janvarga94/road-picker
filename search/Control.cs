using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadPicker.search
{
    class Control
    {
        public static void RecudeColors() {
            var bmp = Model.img;

            var neuralNet = new NeuralSomething(3, 3, 60 * 60);
            neuralNet.DoWhatUrMadeFor(bmp);


            for (int i = 0; i < bmp.Width; i++) {
                for (int j = 0; j < bmp.Height; j++)
                {
                    var pixel = bmp.GetPixel(i, j);
                    var winner = neuralNet.findWinner(new jColor(pixel.R,pixel.G,pixel.B));
                    var winnerColor = neuralNet.getColorAt(winner.X,winner.Y);
      
                    var newColor = Color.FromArgb((int) winnerColor.R, (int)winnerColor.G, (int)winnerColor.B);

             //       Console.WriteLine("before error");
                    bmp.SetPixel(i,j,newColor);
              //      Console.WriteLine("after error");
                }
            }

            new NeuralShower(neuralNet).Show();
            MainWindow.instance.loadImage(bmp);
        }
    }
}
