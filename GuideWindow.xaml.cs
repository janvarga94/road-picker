using RoadPicker.search;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for GuideWindow.xaml
    /// </summary>
    public partial class GuideWindow : Window, INotifyPropertyChanged
    {
        private Color _hoverColor;
        public Color hoverColor
        {
            get
            {
                return _hoverColor;
            }
            set
            {
                _hoverColor = value;
                OnPropertyChanged(nameof(hoverColor));
            }                  
        }

        public double StepMultiplier
        {
            get
            {
                return Model.stepMultiplier;
            }
            set
            {
                if (value > slider.Maximum) value = slider.Maximum;
                if (value < slider.Minimum) value = slider.Minimum;
                Model.stepMultiplier = value;
                Model.stepRadius = value * Model.stepMax;
                OnPropertyChanged(nameof(StepMultiplier));
            }
        }

        

        public GuideWindow()
        {
            InitializeComponent();
            DataContext = this;

            hoverColor = Colors.Black;
        }

        #region propertyHandler
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var p = e.GetPosition(this);

            Left = Left + p.X + 10;
            Top = Top + p.X + 10;
        }

    /*    public void addTolerantColor(Color c) {
            Canvas canvy = new Canvas();
            canvy.Background = new SolidColorBrush(c);
            canvy.Height = 20;
            canvy.Width = 20;

            Border border = new Border();
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            border.Child = canvy;


            addedColors.Children.Add(border);
        }*/
        public void addTolerantColor() {
            Canvas canvy = new Canvas();
            canvy.Background = new SolidColorBrush(_hoverColor);
            canvy.Height = 20;
            canvy.Width = 20;

            Border border = new Border();
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            border.Child = canvy;


            addedColors.Children.Add(border);
            Model.TolerantColors.Add(_hoverColor);
        }
        public void clearTolerantColorPanel()
        {
            addedColors.Children.Clear();
            //TODO tolerancija za algoritam pretrage nije implementirana
        }
    }
}
