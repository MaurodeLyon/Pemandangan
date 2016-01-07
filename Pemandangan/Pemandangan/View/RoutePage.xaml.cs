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

        public List<Route> RouteList { get; set; } = new List<Route>();

        private Frame myframe;
        private DataHandler datandler;

        public RoutePage()
        {
            this.InitializeComponent();

            //test routes
            datandler = new DataHandler();
            datandler.LoadRoutes();

            Route route = datandler.lastRoute;

            this.RouteList.Add(route);
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            Route route = (Route) e.ClickedItem;
            myframe.Navigate(typeof(MapPage), new RouteWrapper(myframe, datandler));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            myframe = (Frame) e.Parameter;

        }
    } 
}
