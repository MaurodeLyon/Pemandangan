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
        Model.PlaceInfo info = new Model.PlaceInfo();

        private Waypoint waypoint;
        private Frame frame;

        public InfoPage()
        {
            this.InitializeComponent();
            try
            {
                Picture.Source = info.getPlace(1);
            }
            catch (NullReferenceException ex)
            {

            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Tuple<Waypoint, Frame> data = (Tuple<Waypoint, Frame>)e.Parameter;
            waypoint = data.Item1;
            frame = data.Item2;

            if (e.Parameter != null)
            {
                if (waypoint.image != null)
                {
                    Picture.Source = new BitmapImage(new System.Uri(waypoint.image, UriKind.Absolute));
                }
                Titel.Text = waypoint.name;
                InfoText.Text = waypoint.description;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (frame.CanGoBack)
                frame.GoBack();
        }
    }
}
