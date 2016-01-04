using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
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

        public MapPage()
        {
            this.InitializeComponent();
            walkedRoute = new List<Geopoint>();
            loadAssets();
        }

        private async void loadAssets()
        {
            var uri1 = new System.Uri("ms-appx:///Assets/test01.jpg");
            var uri2 = new System.Uri("ms-appx:///Assets/test01.jpg");
            waypoint = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri1);
            seen = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri2);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

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
            currentPos.ZIndex = 4;

            map.MapElements.Add(currentPos);
            
            await map.TrySetViewAsync(pos, 17);

            setupGeofencing();
            setupGeofences();
        }

        private void setupGeofences()
        {
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



            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
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
                            var dialog = new Windows.UI.Popups.MessageDialog(geofence.Id + "Entered");
                            var result = await dialog.ShowAsync();
                        }
                            



                    }

                    else if (state == GeofenceState.Exited)
                    {

                    }
                }
            });
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
                test = two.Title + args.MapElements.Count;
            }

            var dialog = new Windows.UI.Popups.MessageDialog(
                "Aliquam laoreet magna sit amet mauris iaculis ornare. " +
                "Morbi iaculis augue vel elementum volutpat.",
                "Lorem Ipsum" + test);

            var result = await dialog.ShowAsync();
        }

        public async void buildMap(List<Geopoint> list)
        {
            MapRouteFinderResult routeResult
                = await MapRouteFinder.GetDrivingRouteFromWaypointsAsync(list);

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

            line.Path = new Geopath(b.Path.Positions);

            map.MapElements.Add(line);
        }

    }
}
