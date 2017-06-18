using System;
using System.Collections.Generic;
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

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
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
            }
        }

        private int ReadActionInterval()
        {
            string actionIntervalText = this.cbActionDigit3.SelectionBoxItem.ToString() + this.cbActionDigit2.SelectionBoxItem.ToString() + this.cbActionDigit1.SelectionBoxItem.ToString();
            return int.Parse(actionIntervalText);
        }

        private int ReadPauseInterval()
        {
            string actionIntervalText = this.cbPauseDigit3.SelectionBoxItem.ToString() + this.cbPauseDigit2.SelectionBoxItem.ToString() + this.cbPauseDigit1.SelectionBoxItem.ToString();
            return int.Parse(actionIntervalText);
        }
    }
}
