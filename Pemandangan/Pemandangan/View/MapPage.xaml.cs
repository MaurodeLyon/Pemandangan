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
using Windows.UI.Popups;
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

        private Frame frame;
        private Route route;
        private Uri uri1;
        private Uri uri2;
        private Uri person;
        private bool mapBuild = false;
        private DataHandler dataHandler;
        private bool infoOpen = false;

        public MapPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            walkedRoute = new List<Geopoint>();
            loadAssets();
            geolocator = new Geolocator
            {
                DesiredAccuracy = PositionAccuracy.High,
                MovementThreshold = 1
            };
            geolocator.PositionChanged += GeolocatorPositionChanged;
            geolocator.StatusChanged += Geolocator_StatusChanged;
        }

        private void Geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            string lang = (string)LanguagePage.LOCAL_SETTINGS.Values["Language"];
            switch (args.Status)
            {
                case PositionStatus.Ready:
                    break;
                case PositionStatus.Initializing:
                    break;
                case PositionStatus.NoData:
                    if (lang == "en")
                    {
                        pushNot("GPS Status", "Can't receive GPS data.");
                    }
                    else if (lang == "nl")
                    {
                        pushNot("Kan geen GPS data binnen krijgen.", "GPS Status");
                    }
                    else
                    {
                        pushNot("Can't receive GPS data.", "GPS Status");
                    }
                    
                    break;
                case PositionStatus.Disabled:
                    if (lang == "en")
                    {
                        pushNot("GPS is disabled. Turn on GPS and reselect your route.", "GPS Status");
                    }
                    else if (lang == "nl")
                    {
                        pushNot("GPS is uitgeschakeld. schakel GPS in en herselecteer uw route", "GPS Status");
                    }
                    else
                    {
                        pushNot("GPS is disabled. Turn on GPS and reselect your route.", "GPS Status");
                    }
                    break;
                case PositionStatus.NotInitialized:
                    break;
                case PositionStatus.NotAvailable:
                    if (lang == "en")
                    {
                        pushNot("Can't receive GPS data.", "GPS Status");
                    }
                    else if (lang == "nl")
                    {
                        pushNot("Kan geen GPS data binnen krijgen.", "GPS Status");
                    }
                    else
                    {
                        pushNot("Can't receive GPS data.", "GPS Status");
                    }
                    break;
                default:
                    break;
            }
        }

        private void loadAssets()
        {
            uri1 = new System.Uri("ms-appx:///Assets/NotSeenWayPoint.png");
            uri2 = new System.Uri("ms-appx:///Assets/SeenWayPoint.png");
            person = new Uri("ms-appx:///Assets/Person.png");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Tuple<Frame, DataHandler> data = (Tuple<Frame, DataHandler>)e.Parameter;
            frame = data.Item1;
            if (data.Item2 != null)
                dataHandler = data.Item2;

            if (geolocator.LocationStatus == PositionStatus.Ready && !infoOpen)
            {
                drawCurrentPosition();
                if (!mapBuild && dataHandler != null)
                {
                    route = dataHandler.lastRoute;
                    setupGeofencing();
                    buildMap();
                }

                if (infoOpen)
                    infoOpen = false;
                
            }
        }

        private async void drawCurrentPosition()
        {
            Geoposition d = await geolocator.GetGeopositionAsync();
            Geopoint pos = new Geopoint(d.Coordinate.Point.Position);
            currentPos = new MapIcon();
            currentPos.Location = pos;
            currentPos.NormalizedAnchorPoint = new Point(0.5, 1.0);
            currentPos.Title = "Current position";
            currentPos.ZIndex = 5;
            currentPos.Image = await StorageFile.GetFileFromApplicationUriAsync(person);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                map.MapElements.Add(currentPos);
            });
            await map.TrySetViewAsync(pos, 17);
        }

        public async void setupGeofencing()
        {
            GeofenceMonitor.Current.Geofences.Clear();
            Geoposition location = await geolocator.GetGeopositionAsync().AsTask();
            Geofence fence = GeofenceMonitor.Current.Geofences.FirstOrDefault(gf => gf.Id == "currentLoc");

            if (fence == null)
                GeofenceMonitor.Current.Geofences.Add(
                     new Geofence("currentLoc",
                     new Geocircle(location.Coordinate.Point.Position, 10.0),
                     MonitoredGeofenceStates.Entered,
                     false,
                     TimeSpan.FromSeconds(10)));

            GeofenceMonitor.Current.GeofenceStateChanged += GeofenceStateChanged;
        }

        private async void GeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            var reports = sender.ReadReports();

            foreach (GeofenceStateChangeReport report in reports)
            {
                GeofenceState state = report.NewState;
                Geofence geofence = report.Geofence;

                switch (state)
                {
                    case GeofenceState.None:
                        break;
                    case GeofenceState.Entered:
                        if (geofence.Id != "currentLoc")
                        {
                            string desc = "";
                            foreach (Waypoint e in route.waypoints)
                                if (geofence.Id == e.name)
                                {
                                    desc = e.name;
                                    pushNot("Waypoint Nearby", desc);
                                    if (geofence.Id == "Einde stadswandeling")
                                    {
                                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                        {
                                            map.MapElements.Clear();
                                            walkedRoute = new List<Geopoint>();
                                            map.MapElements.Add(currentPos);
                                        });
                                        return;
                                    }
                                }
                            
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                            {
                                foreach (MapElement me in map.MapElements)
                                    if (me is MapIcon)
                                    {
                                        MapIcon icon = (MapIcon)me;
                                        if (icon.Title == geofence.Id)
                                            icon.Image = await StorageFile.GetFileFromApplicationUriAsync(uri2);
                                    }
                            });
                        }
                        break;
                    case GeofenceState.Exited:
                        break;
                    case GeofenceState.Removed:
                        break;
                    default:
                        break;
                }
            }
        }

        private async void GeolocatorPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Geoposition geoposition = await geolocator.GetGeopositionAsync();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                currentPos.Location = geoposition.Coordinate.Point;
            });
            walkedRoute.Add(geoposition.Coordinate.Point);
            drawWalkedRoute();
            await map.TrySetViewAsync(geoposition.Coordinate.Point, 17);
        }



        public async void drawWalkedRoute()
        {
            if (walkedRoute.Count >= 2 && dataHandler != null)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MapPolyline walkedLine = new MapPolyline
                    {
                        StrokeThickness = 11,
                        StrokeColor = Colors.Gray,
                        StrokeDashed = false,
                        ZIndex = 3
                    };

                    List<BasicGeoposition> tempList = new List<BasicGeoposition>();

                    foreach (Geopoint e in walkedRoute)
                        tempList.Add(e.Position);

                    walkedLine.Path = new Geopath(tempList);
                    if (LatestwalkedLine != null)
                        map.MapElements.Remove(LatestwalkedLine);

                    LatestwalkedLine = walkedLine;
                    map.MapElements.Add(LatestwalkedLine);
                });
            }
        }

        private void map_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            string desc = "Nothing";
            if (args.MapElements.First() is MapIcon)
            {
                MapIcon mapIcon = (MapIcon)args.MapElements.First();
                desc = mapIcon.Title;
            }

            if (desc != "Current position")
                foreach (Waypoint w in route.waypoints)
                    if (w.name == desc)
                    {
                        frame.Navigate(typeof(InfoPage), new Tuple<Waypoint, Frame>(w, frame));
                        infoOpen = true;

                    }

        }

        public void buildMap()
        {
            List<Geopoint> landMarkList = new List<Geopoint>();

            foreach (Waypoint e in route.waypoints)
            {
                landMarkList.Add(e.GeoPosition());
                if (e.landmark)
                {
                    addMapIcon(e);
                    setupGeofences(e);
                }
            }
            getMap(landMarkList);
            mapBuild = true;
        }

        private async void getMap(List<Geopoint> list)
        {
            MapRouteFinderResult routeResult = await MapRouteFinder.GetWalkingRouteFromWaypointsAsync(list);
            MapRoute mapRoute = routeResult.Route;

            MapPolyline line = new MapPolyline
            {
                StrokeThickness = 11,
                StrokeColor = Colors.Green,
                StrokeDashed = false,
                ZIndex = 2
            };

            if (mapRoute != null)
                line.Path = new Geopath(mapRoute.Path.Positions);
            else
            {
                getMap(list);
                return;
            }
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                map.MapElements.Add(line);
            });
        }

        private async void addMapIcon(Waypoint e)
        {
            MapIcon m = new MapIcon();
            m.Location = e.GeoPosition();
            m.NormalizedAnchorPoint = new Point(0.5, 1.0);
            m.Title = e.name;
            m.ZIndex = 4;
            m.Image = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri1);

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                map.MapElements.Add(m);
            });
        }



        private void setupGeofences(Waypoint w)
        {
            Geocircle geocircle = new Geocircle(w.GeoPosition().Position, 40);
            MonitoredGeofenceStates mask = MonitoredGeofenceStates.Entered | MonitoredGeofenceStates.Exited;
            Geofence fence = new Geofence(w.name, geocircle, mask, false, new TimeSpan(0));
            if (!GeofenceMonitor.Current.Geofences.Contains(fence))
                GeofenceMonitor.Current.Geofences.Add(fence);
        }

        public void pushNot(string title, string desc)
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
