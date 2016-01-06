using Pemandangan.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Xml.Dom;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Pemandangan.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapPage : Page
    {
        private Geolocator geolocator;
        private MapIcon currentPos;
        private MapPolyline LatestwalkedLine;
        private List<Geopoint> walkedRoute;

        private StorageFile waypoint;
        private StorageFile seen;
        private Route route;
        private RouteWrapper wrap;
        private Uri uri1;
        private Uri uri2;
        private bool mapBuild = false;

        public MapPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            walkedRoute = new List<Geopoint>();
            loadAssets();
        }

        private async void loadAssets()
        {
            uri1 = new System.Uri("ms-appx:///Assets/WaypointRed.png");
            uri2 = new System.Uri("ms-appx:///Assets/WaypointGreen.png");
            waypoint = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri1);
            seen = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri2);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            wrap = (RouteWrapper)e.Parameter;
            route = wrap.route;
            if (!mapBuild)
            {
                setupGeofencing();
                buildMap();
                if (geolocator == null)
                {
                    geolocator = new Geolocator
                    {
                        DesiredAccuracy = PositionAccuracy.High,
                        MovementThreshold = 1
                    };

                    geolocator.PositionChanged += GeolocatorPositionChanged;
                    //GeofenceMonitor.Current.GeofenceStateChanged += GeofenceStateChanged;
                }
                Geoposition d = await geolocator.GetGeopositionAsync();

                var pos = new Geopoint(d.Coordinate.Point.Position);

                currentPos = new MapIcon();
                currentPos.Location = pos;
                currentPos.NormalizedAnchorPoint = new Point(0.5, 1.0);
                currentPos.Title = "Current position";
                currentPos.ZIndex = 5;
                currentPos.Image = waypoint;
                map.MapElements.Add(currentPos);

                await map.TrySetViewAsync(pos, 17);
            }

            
            
        }

        

        public async void setupGeofencing()
        {
            GeofenceMonitor.Current.Geofences.Clear();
            Geolocator locator = new Geolocator();

            var location = await locator.GetGeopositionAsync().AsTask();

            Geofence fence
             = GeofenceMonitor.Current.Geofences.FirstOrDefault(gf => gf.Id == "currentLoc");

            if (fence == null)
            {
                GeofenceMonitor.Current.Geofences.Add(
                     new Geofence("currentLoc", new Geocircle(location.Coordinate.Point.Position, 10.0), MonitoredGeofenceStates.Entered,
                                    false, TimeSpan.FromSeconds(10))
                        );
            }
            GeofenceMonitor.Current.GeofenceStateChanged += GeofenceStateChanged;
        }

        private async void GeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            var reports = sender.ReadReports();



            
                foreach (GeofenceStateChangeReport report in reports)
                {
                    GeofenceState state = report.NewState;
                    Geofence geofence = report.Geofence;

                    if (state == GeofenceState.Removed)
                    {

                    }

                    else if (state == GeofenceState.Entered)
                    {

                        if (geofence.Id != "currentLoc")
                        {
                        //var dialog = new Windows.UI.Popups.MessageDialog(geofence.Id + " Entered");
                        //var result = await dialog.ShowAsync();
                        String desc="";

                        foreach(Waypoint e in route.waypoints)
                        {
                            if(geofence.Id == e.name)
                            {
                                desc = e.name;
                            }
                        }
                            
                            pushNot("Waypoint Nearby", desc);
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,async () =>
                        {
                            foreach (MapElement me in map.MapElements)
                            {
                                if (me is MapIcon)
                                {
                                    MapIcon icon = (MapIcon)me;
                                    if (icon.Title == geofence.Id)
                                    {

                                        icon.Image = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri2);
                                    }
                                }
                            }
                        });
                        }




                    }

                    else if (state == GeofenceState.Exited)
                    {

                    }
                }
           
        }

        private async void GeolocatorPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Geoposition d = await geolocator.GetGeopositionAsync();

            var pos = new Geopoint(d.Coordinate.Point.Position);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                currentPos.Location = pos;

            });
            walkedRoute.Add(pos);
            drawWalkedRoute();
            await map.TrySetViewAsync(pos, 17);
        }

        public async void drawWalkedRoute()
        {


            if (walkedRoute.Count >= 2)
            {


                //MapRouteFinderResult routeResult
                //   = await MapRouteFinder.GetWalkingRouteFromWaypointsAsync(walkedRoute);

                //MapRoute b = routeResult.Route;


                var color = Colors.Gray;
                //color.A = ;
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var walkedLine = new MapPolyline
                    {
                        StrokeThickness = 11,
                        StrokeColor = color,
                        StrokeDashed = false,
                        ZIndex = 3


                    };

                    List<BasicGeoposition> tempList = new List<BasicGeoposition>();

                    foreach (Geopoint e in walkedRoute)
                    {
                        tempList.Add(e.Position);
                    }

                    //walkedLine.Path = new Geopath(b.Path.Positions);
                    walkedLine.Path = new Geopath(tempList);
                    if (LatestwalkedLine != null)
                    {
                        map.MapElements.Remove(LatestwalkedLine);
                        LatestwalkedLine = walkedLine;
                    }
                    else
                    {
                        LatestwalkedLine = walkedLine;
                    }
                    map.MapElements.Add(LatestwalkedLine);
                });




            }
        }


        private async void map_MapElementClick(Windows.UI.Xaml.Controls.Maps.MapControl sender, Windows.UI.Xaml.Controls.Maps.MapElementClickEventArgs args)
        {
            String test="Nothing";
            if (args.MapElements.First() is MapIcon)
            {
                MapIcon two = (MapIcon)args.MapElements.First();
                test = two.Title;
            }
            string desc = "";

            foreach(Waypoint w in route.waypoints)
            {
                if (w.name == test)
                {
                    desc = w.description;

                    wrap.frame.Navigate(typeof(InfoPage), new WaypointWrapper(w,wrap.frame));
                }
            }

            if(args.MapElements.First() is MapIcon)
            {
                pushNot("Waypoint nearby!", test);
            }
           // var dialog = new Windows.UI.Popups.MessageDialog(
              //  test + "'\n"+ desc);

            //var result = await dialog.ShowAsync();
        }

        public async void buildMap()
        {

            List<Geopoint> tempList= new List<Geopoint>();
            int i =GeofenceMonitor.Current.Geofences.Count;

            foreach (Waypoint e in route.waypoints)
            {
                tempList.Add(e.GeoPosition());
                if(e.landmark)
                {
                    addMapIcon(e);
                    setupGeofences(e);
                }
                
            }


            getMap(tempList);
            mapBuild = true;

        }

        private async void getMap(List<Geopoint> list)
        {

            MapRouteFinderResult routeResult
                = await MapRouteFinder.GetWalkingRouteFromWaypointsAsync(list);

            MapRoute b = routeResult.Route;


            var color = Colors.Green;
            color.A = 128;

            var line = new MapPolyline
            {
                StrokeThickness = 11,
                StrokeColor = color,
                StrokeDashed = false,
                ZIndex = 2
            };

            if(b!=null)
            {
                line.Path = new Geopath(b.Path.Positions);
            }
            else
            {
                getMap(list);
                return;
            }

            map.MapElements.Add(line);
        }

        private async void addMapIcon(Waypoint e)
        {
            MapIcon m = new MapIcon();
            m.Location = e.GeoPosition();
            m.NormalizedAnchorPoint = new Point(0.5, 1.0);
            m.Title = e.name;
            m.ZIndex = 4;
            m.Image = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri1); ;

            map.MapElements.Add(m);
        }
        private void setupGeofences(Waypoint w)
        {
            Geocircle geocircle = new Geocircle(w.GeoPosition().Position, 40);
            MonitoredGeofenceStates mask = MonitoredGeofenceStates.Entered | MonitoredGeofenceStates.Exited;
            Geofence fence = new Geofence(w.name, geocircle, mask, false, new TimeSpan(0));
            if(!GeofenceMonitor.Current.Geofences.Contains(fence))
            GeofenceMonitor.Current.Geofences.Add(fence);
        }

        public void pushNot(String title, String desc)
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(title));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(desc));

            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            XmlElement test = toastXml.CreateElement("test");

            test.SetAttribute("src", "ms-winsoundevent:Notification.IM");

            toastNode.AppendChild(test);

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);


        }

    }
}
