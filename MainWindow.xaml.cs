using RoadPicker.search;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System;
using System.Collections.Generic;
using ScreenShotDemo;
using System.Windows.Threading;
using RoadPicker.help_classes;
using System.Linq;
using System.Threading;
using System.ComponentModel;

namespace RoadPicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {
        public static MainWindow instance;

        public GuideWindow guideWindow;
        public StartGuideCloud startGuide;

        private BitmapImage smallPlaceholder = new BitmapImage(new Uri(@"images\small_placeholder.png", UriKind.Relative));
        private BitmapImage bigPlaceholder = new BitmapImage(new Uri(@"images\bigg_placeholder.png", UriKind.Relative));
        private BitmapImage xMark = new BitmapImage(new Uri(@"images\dot20.png", UriKind.Relative));

        private Random rnd = new Random();
        private BackgroundWorker worker = new BackgroundWorker() { WorkerSupportsCancellation = true };

        public MainWindow()
        {

            InitializeComponent();
            this.DataContext = this;

            /*  var neuralNet = new NeuralSomething(5, 5, 60 * 60);
              neuralNet.DoWhatUrMadeFor(new Bitmap(@"asdf2.jpg"));
              new NeuralShower(neuralNet).Show();*/

            instance = this;
            screenCapture();
            image.SizeChanged += Image_SizeChanged;

            guideWindow = new GuideWindow();
            startGuide = new StartGuideCloud();
            startGuide.Show();
            //keylogger
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                KeyboardHook.CreateHook();
                KeyboardHook.KeyPressed += (sender, e) =>
                {
                    switch (e.KeyCode)
                    {
                        case System.Windows.Forms.Keys.F5: //toogle
                            toogleShowWindowAndCapture();
                            break;
                        case System.Windows.Forms.Keys.F6: //search
                            searchPath();
                            break;
                        case System.Windows.Forms.Keys.F7: //reset
                            resetSearch();
                            break;
                        case System.Windows.Forms.Keys.F8: //close app                         
                            guideWindow.addTolerantColor();
                            break;
                        case System.Windows.Forms.Keys.F9: // add tolerant color
                            Application.Current.Shutdown();
                            break;
                        case System.Windows.Forms.Keys.F4:
                            //         search.Control.RecudeColors();
                            break;
                        case System.Windows.Forms.Keys.Add:
                            guideWindow.StepMultiplier += 0.1;
                            break;
                        case System.Windows.Forms.Keys.Subtract:
                            guideWindow.StepMultiplier -= 0.1;
                            break;
                        case System.Windows.Forms.Keys.F2:
                            worker.CancelAsync();
                            break;
                    }
                };

                //       KeyboardHook.DisposeHook();
            }), DispatcherPriority.ContextIdle);


        }


        public void drawDot(System.Windows.Point p, double radius, System.Windows.Media.SolidColorBrush brush = null)
        {
            var ellipse = new Ellipse() { Width = radius / 2, Height = radius / 2, Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 3 };
            ellipse.Stroke = brush ?? System.Windows.Media.Brushes.Yellow;
            Canvas.SetLeft(ellipse, p.X - radius / 2);
            Canvas.SetTop(ellipse, p.Y - radius / 2);
            canvas.Children.Add(ellipse);
        }
        public void drawPlaceholder(System.Windows.Point p)
        {
            System.Windows.Controls.Image imgggg = new System.Windows.Controls.Image
            {
                Source = bigPlaceholder
            };
            Canvas.SetLeft(imgggg, p.X - imgggg.Source.Width / 2);
            Canvas.SetTop(imgggg, p.Y - imgggg.Source.Height);
            canvas.Children.Add(imgggg);
        }
        public void drawSmallPlaceholder(System.Windows.Point p)
        {
            System.Windows.Controls.Image imgggg = new System.Windows.Controls.Image
            {
                Source = smallPlaceholder
            };
            Canvas.SetLeft(imgggg, p.X - imgggg.Source.Width / 2);
            Canvas.SetTop(imgggg, p.Y - imgggg.Source.Height);
            canvas.Children.Add(imgggg);
        }
        public void drawXMarkPlaceholder(System.Windows.Point p)
        {
            System.Windows.Controls.Image imgggg = new System.Windows.Controls.Image
            {
                Source = xMark
            };
            Canvas.SetLeft(imgggg, p.X - imgggg.Source.Width / 2);
            Canvas.SetTop(imgggg, p.Y - imgggg.Source.Height / 2);

            imgggg.LayoutTransform = new RotateTransform() { Angle = rnd.Next(0, 360) };

            canvas.Children.Add(imgggg);
        }

        private void Image_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            System.Windows.Size size = e.NewSize;
            Model.actualImageHeight = size.Height;
            Model.actualImageWidth = size.Width;
        }

        private Line lineWeHandle = null;  //we will use right mouse click to drag line
        private Line lineFallowinglineWeHandle = null; //this is line that is color contrast to lineWeHandle, cuz we dont know what color of surface will be when user draw lines
        private System.Windows.Point lineWeHandleStartPosition;

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(image);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Model.startState == null)
                {
                    Model.startState = new State { position = p };
                    Model.roadColor = Model.pointToColor(p);
                    //  drawDot(p, 5);
                    drawPlaceholder(p);
                }
                else if (Model.endState == null)
                {
                    Model.endState = new State { position = p };
                    drawPlaceholder(p);
                }

                guideWindow.Activate();
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                lineWeHandleStartPosition = p;
                lineWeHandle = new Line();
                lineWeHandle.X1 = lineWeHandle.X2 = p.X - 2;
                lineWeHandle.Y1 = lineWeHandle.Y2 = p.Y - 2;
                lineWeHandle.Stroke = System.Windows.Media.Brushes.White;
                lineWeHandle.StrokeThickness = 2;


                lineFallowinglineWeHandle = new Line();
                lineFallowinglineWeHandle.X1 = lineFallowinglineWeHandle.X2 = p.X + 2;
                lineFallowinglineWeHandle.Y1 = lineFallowinglineWeHandle.Y2 = p.Y + 2;
                lineFallowinglineWeHandle.Stroke = System.Windows.Media.Brushes.Black;
                lineFallowinglineWeHandle.StrokeThickness = 2;



                canvasWalls.Children.Add(lineWeHandle);
                canvas.Children.Add(lineFallowinglineWeHandle);
            }

        }
        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(image);

            //here we do form that tells whats going on
            guideWindow.Show();
            //we set position relative to mouse position
            guideWindow.Left = p.X + 10;
            guideWindow.Top = p.Y + 10;

            if (e.RightButton == MouseButtonState.Pressed) //if we are draging to draw line
            {
                lineWeHandle.X2 = p.X;
                lineWeHandle.Y2 = p.Y;
                try
                {
                    lineFallowinglineWeHandle.X2 = p.X + 2;
                    lineFallowinglineWeHandle.Y2 = p.Y + 2;
                }
                catch (Exception ee)
                {
                    Console.WriteLine("jan execption: " + ee);
                }
            }

            //also we want to update color we hover to guideWindow
            var color = Model.pointToColor(p);
            guideWindow.hoverColor = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
        private void image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //lineWeHandle = null;
            lineFallowinglineWeHandle = null;
        }
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                if (startGuide.IsLoaded)
                {
                    startGuide.Hide();
                }

                screenCapture();
                WindowState = WindowState.Maximized;
            }
        }
        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                guideWindow.StepMultiplier += 0.1;
            }
            else if (e.Delta < 0)
            {
                guideWindow.StepMultiplier -= 0.1;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"> position relative to image</param>


        private void toogleShowWindowAndCapture()
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Minimized;
                guideWindow.Hide();
                if (startGuide.IsLoaded)
                {
                    startGuide.Show();
                }

            }
            else
            {
                if (startGuide.IsLoaded)
                {
                    startGuide.Hide();
                }

                screenCapture();
                WindowState = WindowState.Maximized;
                guideWindow.Show();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"> for example C:\bla\bla.txt</param>
        public void loadImage(String path)
        {
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri(path, UriKind.Relative);
            bi3.EndInit();
            Console.WriteLine(bi3.Width);
            image.Source = bi3;
            Model.img = new Bitmap(path);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="picture"></param>
        public void loadImage(System.Drawing.Image picture)
        {
            var bitmap = new Bitmap(picture);
            IntPtr bmpPt = bitmap.GetHbitmap();
            BitmapSource bitmapSource =
             System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                   bmpPt,
                   IntPtr.Zero,
                   Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());

            //freeze bitmapSource and clear memory to avoid memory leaks
            bitmapSource.Freeze();

            image.Source = bitmapSource;
            Model.img = new Bitmap(picture);

            //here we test neural thing algor.
        }


        private void screenCapture()
        {
            ScreenCapture sc = new ScreenCapture();
            System.Drawing.Image img = sc.CaptureScreen();
            loadImage(img);
        }
        private void searchPath()
        {
            if (Model.startState != null && Model.endState != null)
            {
                Model.prepareForSearch();
                Cursor = System.Windows.Input.Cursors.Wait;


                worker.DoWork += runSearch;
                worker.RunWorkerCompleted += afterSearch;
                worker.RunWorkerAsync();

            }
        }

        private void runSearch(object sender, DoWorkEventArgs e)
        {
            var algorithm = new ConcurrentAStarSearch();
            DateTime time = DateTime.Now;
            State result = algorithm.search(Model.startState, e, worker);
            e.Result = result;
        }
        private void afterSearch(object sender, RunWorkerCompletedEventArgs e)
        {
            worker.DoWork -= runSearch;
            worker.RunWorkerCompleted -= afterSearch;

            if (e.Cancelled)
            {
                Cursor = System.Windows.Input.Cursors.Arrow;
                return;
            }
            var result = (State)e.Result;
            if (result == null)
            {
                Console.WriteLine("Couldn't find path :/");
                System.Drawing.Point p = System.Windows.Forms.Cursor.Position;
                new SearchResultMessage(false, p.X, p.Y);
            }
            else
            {

                // Console.WriteLine("Path found in : {0}", (DateTime.Now - time).TotalSeconds);
                System.Drawing.Point p = System.Windows.Forms.Cursor.Position;
                new SearchResultMessage(true, p.X, p.Y);
                List<State> path = result.path();
                foreach (State st in path)
                {
                    drawXMarkPlaceholder(st.position);
                }
            }
            Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void resetSearch()
        {
            canvas.Children.Clear();
            canvasWalls.Children.Clear();
            guideWindow.clearTolerantColorPanel();
            Model.startState = Model.endState = null;
            Model.TolerantColors.Clear();
        }


    }
}
