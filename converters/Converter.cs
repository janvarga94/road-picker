using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadPicker.converters
{
    class Converter
    {
        public static System.Windows.Media.Color colorSwitch( System.Drawing.Color color) {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
        public static System.Drawing.Color colorSwitch(System.Windows.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
