using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LooperSolution
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer dt = new DispatcherTimer();
        bool isLooperRunning = false;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            if (isLooperRunning)
            {
                this.dt.Stop();
                this.isLooperRunning = !this.isLooperRunning;
                this.btnStartStop.Content = "Start";
            }
            else
            {
                var pauseInterval = this.ReadPauseInterval();
                var actionInterval = this.ReadActionInterval();
                MediaElement player = new MediaElement();
                Windows.Storage.StorageFolder assetsFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
                Windows.Storage.StorageFile sound = await assetsFolder.GetFileAsync("boom.wav");
                var soundStream = await sound.OpenReadAsync();
                player.SetSource(soundStream, "");

                if (actionInterval > 0)
                {
                    this.dt.Interval = TimeSpan.FromSeconds(actionInterval);
                    this.dt.Tick += async (dtSender, dtEvents) =>
                    {
                        player.Play();
                        await Task.Delay(TimeSpan.FromSeconds(pauseInterval));
                        player.Play();
                    };

                    this.dt.Start();
                    this.isLooperRunning = !this.isLooperRunning;
                    this.btnStartStop.Content = "Stop";
                }
            }
        }

        private int ReadActionInterval()
        {
            string actionIntervalText = this.cbActionDigit3.SelectionBoxItem.ToString() + this.cbActionDigit2.SelectionBoxItem.ToString() + this.cbActionDigit1.SelectionBoxItem.ToString();
            return int.Parse(actionIntervalText);
        }

        private int ReadPauseInterval()
        {
            //string actionIntervalText = this.cbPauseDigit3.SelectionBoxItem.ToString() + this.cbPauseDigit2.SelectionBoxItem.ToString() + this.cbPauseDigit1.SelectionBoxItem.ToString();
            return 5;// int.Parse(actionIntervalText);
        }

        double pause = 0;
        double pause2 = 0;
        double pause3= 0;

        private void ContentControl_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
        }

        private void ContentControl_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            pause += Math.Round(Math.Abs(e.Delta.Translation.Y) * e.Velocities.Linear.Y *10);
            pause2 += Math.Round(Math.Abs(e.Cumulative.Translation.Y) * e.Velocities.Linear.Y);
            pause3 += e.Velocities.Linear.Y * 5;

            Debug.WriteLine("pause: {4}, e.Delta.Translation.Y: {0}, e.Cumulative.Translation.Y: {1}, Velocities: {2}, Position.Y: {3}", 
                e.Delta.Translation.Y, 
                e.Cumulative.Translation.Y,
                e.Velocities.Linear.Y,
                e.Position.Y,
                pause);

            if (pause < 0)
                pause = 0;

            if (pause2 < 0)
                pause2 = 0;

            if (pause3 < 0)
                pause3 = 0;

            if (pause > 3599)
                pause = 3599;

            if (pause2 > 3599)
                pause2 = 3599;

            if (pause3 > 3599)
                pause3 = 3599;

            TimeSpan ts = TimeSpan.FromSeconds(pause);
            this.pauseText.Text = ts.ToString(@"mm\:ss");
            
            TimeSpan ts2 = TimeSpan.FromSeconds(pause2);
            this.pauseText2.Text = ts2.ToString(@"mm\:ss");

            TimeSpan ts3 = TimeSpan.FromSeconds(pause3);

            string timeTextFormat = string.Empty;

            if (ts3.Minutes > 0)
            {
                timeTextFormat += "{0:%m} min ";
            } 
            if (ts3.Seconds != 0 || ts3.Minutes == 0)
            {
                timeTextFormat += "{0:%s} sec";
            }

            //this.pauseText3.Text = string.Format(timeTextFormat, ts3);

            // slider
            var angle = GetAngle(e.Position, (sender as ContentControl).RenderSize);

            ((sender as ContentControl).DataContext as ViewModel).Angle = angle;
        }

        private void Grid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var grid = sender as Grid;
        }

        public enum Quadrants : int { nw = 2, ne = 1, sw = 4, se = 3 }
        private double GetAngle(Point touchPoint, Size circleSize)
        {
            var _X = touchPoint.X - (circleSize.Width / 2d);
            var _Y = circleSize.Height - touchPoint.Y - (circleSize.Height / 2d);
            var _Hypot = Math.Sqrt(_X * _X + _Y * _Y);
            var _Value = Math.Asin(_Y / _Hypot) * 180 / Math.PI;
            var _Quadrant = (_X >= 0) ?
                (_Y >= 0) ? Quadrants.ne : Quadrants.se :
                (_Y >= 0) ? Quadrants.nw : Quadrants.sw;
            switch (_Quadrant)
            {
                case Quadrants.ne: _Value = 090 - _Value; break;
                case Quadrants.nw: _Value = 270 + _Value; break;
                case Quadrants.se: _Value = 090 - _Value; break;
                case Quadrants.sw: _Value = 270 + _Value; break;
            }
            return _Value;
        }

        private void ContentControl_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
        }
    }

    public class ViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public ViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Angle = 45;
            }
        }

        double m_Angle = default(double);
        public double Angle
        {
            get { return m_Angle; }
            set
            {
                SetProperty(ref m_Angle, value);
                Value = (int)(value / 6d);
            }
        }

        int m_Value = default(int);
        public int Value { get { return m_Value; } private set { SetProperty(ref m_Value, value); } }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void SetProperty<T>(ref T storage, T value, [System.Runtime.CompilerServices.CallerMemberName] String propertyName = null)
        {
            if (!object.Equals(storage, value))
            {
                storage = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
        protected void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }

}
