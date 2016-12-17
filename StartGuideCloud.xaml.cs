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
    /// Interaction logic for StartGuideCloud.xaml
    /// </summary>
    public partial class StartGuideCloud : Window
    {
        public StartGuideCloud()
        {
            InitializeComponent();

            var h1 = SystemParameters.FullPrimaryScreenHeight;
            var h2 = SystemParameters.PrimaryScreenHeight;

            var w1 = SystemParameters.FullPrimaryScreenWidth;
            var w2 = SystemParameters.PrimaryScreenWidth;

            Left = w1 - Width - 10;
            Top = h1 - 300;

            
           

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
