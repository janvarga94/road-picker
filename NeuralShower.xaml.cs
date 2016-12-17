using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RoadPicker
{
    /// <summary>
    /// Interaction logic for NeuralShower.xaml
    /// </summary>
    public partial class NeuralShower : Window
    {
        public NeuralShower(NeuralSomething neur)
        {
            InitializeComponent();
            DataContext = this;

            var rectWidth = 300 /  neur.Matrix.GetLength(0);
            var rectHeight = 200 / neur.Matrix.GetLength(1);


            for (int i = 0; i < neur.Matrix.GetLength(0); i++) {
                for (int j = 0; j < neur.Matrix.GetLength(1); j++)
                {
                    var color = neur.Matrix[i,j];
                    var rect = new Rectangle();
                    rect.Width = rectWidth;
                    rect.Height = rectHeight;

                    Canvas.SetLeft(rect, i * rectWidth);
                    Canvas.SetTop(rect, j * rectHeight);

                    rect.Fill = new SolidColorBrush(Color.FromRgb((byte)color.R, (byte)color.G, (byte)color.B));

                    canvas.Children.Add(rect);
                }
            }

        }

    }
}
