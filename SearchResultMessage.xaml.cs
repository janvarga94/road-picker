using System;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace RoadPicker
{
    /// <summary>
    /// Interaction logic for SearchResultMessage.xaml
    /// </summary>
    public partial class SearchResultMessage : Window
    {
        public SearchResultMessage(Boolean searchSuccess, int left, int top) //left and top are position of window
        {
            InitializeComponent();
            DataContext = this;

            if (!searchSuccess)
                TextMessage.Text = "✘ Path not found :/";

            //after 3 sec close
            var _tm = new DispatcherTimer();
            _tm.Tick += new EventHandler(_tm_Elapsed);
            _tm.Interval = new TimeSpan(0,0,2);
            _tm.Start();


            Left = left;
            Top = top;

            Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Closing -= Window_Closing;
            e.Cancel = true;
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(0.5));
            anim.Completed += (s, _) => this.Close();
            this.BeginAnimation(UIElement.OpacityProperty, anim);
        }
        void _tm_Elapsed(object sender, EventArgs e)
        {

            ((DispatcherTimer)sender).Stop();
            Close();

        }


        //code down here is to make window hittestvisible false. Doing it in code doesnt work

        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Get this window's handle
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            // Change the extended window style to include WS_EX_TRANSPARENT
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
          
        }
    }
}
