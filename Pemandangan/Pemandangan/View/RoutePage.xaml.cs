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
using Windows.UI.Xaml.Navigation;
using Pemandangan.Model;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Pemandangan.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RoutePage : Page
    {
        public List<Route> RouteList = new List<Route>();
        private Frame InnerFrame;
        private DataHandler dataHandler;

        public RoutePage()
        {
            this.InitializeComponent();
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            dataHandler.selectedRoute = (Route)e.ClickedItem;
            InnerFrame.Navigate(typeof(MapPage), new Tuple<Frame,DataHandler>(InnerFrame,dataHandler));
            dataHandler.routeInProgress = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                Tuple<Frame, DataHandler> parameter = (Tuple<Frame, DataHandler>)e.Parameter;
                InnerFrame = parameter.Item1;
                dataHandler = parameter.Item2;
                RouteList = dataHandler.routes;
            }
        }
    }
}
