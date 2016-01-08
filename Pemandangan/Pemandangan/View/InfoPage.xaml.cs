using Pemandangan.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Pemandangan.View
{

    public sealed partial class InfoPage : Page
    {
        private Waypoint waypoint;
        private Frame innerFrame;

        public InfoPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Tuple<Waypoint, Frame> data = (Tuple<Waypoint, Frame>)e.Parameter;
            waypoint = data.Item1;
            innerFrame = data.Item2;

            if (e.Parameter != null)
            {
                if (waypoint.image != null)
                {
                    Picture.Source = new BitmapImage(new Uri(waypoint.image, UriKind.Absolute));
                }
                Titel.Text = waypoint.name;
                if (waypoint.description != null)
                {
                    InfoText.Text = waypoint.description;
                }
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            innerFrame.Navigate(typeof(MapPage), new Tuple<Frame, DataHandler>(innerFrame, null));
        }
    }
}
