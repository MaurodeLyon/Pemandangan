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
        private DataHandler dataHandler;
        private Frame innerFrame;

        private Uri uri1;
        private Uri uri2;
        private Uri person;

        private bool isInfoPageOpen = false;
        private bool isMapBuild = false;

        private Route selectedRoute;
        private List<Geopoint> walkedRoute;
        private MapPolyline walkedLine;
        private MapIcon currentPos;

        public MapPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            walkedRoute = new List<Geopoint>();
            uri1 = new System.Uri("ms-appx:///Assets/NotSeenWayPoint.png");
            uri2 = new System.Uri("ms-appx:///Assets/SeenWayPoint.png");
            person = new Uri("ms-appx:///Assets/Person.png");
            geolocator = new Geolocator
            {
                DesiredAccuracy = PositionAccuracy.High,
                MovementThreshold = 1
            };
            geolocator.PositionChanged += GeolocatorPositionChanged;
            geolocator.StatusChanged += Geolocator_StatusChanged;
            drawCurrentPosition();
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
                        pushNot("Can't receive GPS data.", "GPS Status");
                    }
                    else if (lang == "nl")
                    {
                        pushNot("GPS Status", "Kan geen GPS data binnen krijgen.");
                    }
                    else
                    {
                        pushNot("GPS Status", "Can't receive GPS data.");
                    }
                    break;
                case PositionStatus.Disabled:
                    if (lang == "en")
                    {
                        pushNot("GPS Status", "GPS is disabled. Turn on GPS.");
                    }
                    else if (lang == "nl")
                    {
                        pushNot("GPS Status", "GPS is uitgeschakeld. schakel GPS in.");
                    }
                    else
                    {
                        pushNot("GPS Status", "GPS is disabled. Turn on GPS.");
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
                        pushNot("GPS Status", "Kan geen GPS data binnen krijgen.");
                    }
                    else
                    {
                        pushNot("GPS Status", "Can't receive GPS data.");
                    }
                    break;
                default:
                    break;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Tuple<Frame, DataHandler> data = e.Parameter as Tuple<Frame, DataHandler>;
            innerFrame = data.Item1;
            if (data.Item2 != null) dataHandler = data.Item2;

            if ((geolocator.LocationStatus == PositionStatus.Initializing || geolocator.LocationStatus == PositionStatus.Ready) && !isInfoPageOpen)
            {
                drawCurrentPosition();
                if (!isMapBuild && dataHandler != null)
                {
                    selectedRoute = dataHandler.selectedRoute;
                    setupGeofencing();
                    buildMap();
                }

                if (isInfoPageOpen)
                    isInfoPageOpen = false;
            }
        }

        private async void drawCurrentPosition()
        {
            currentPos = new MapIcon();
            currentPos.NormalizedAnchorPoint = new Point(0.5, 1.0);
            currentPos.Title = "Current position";
            currentPos.ZIndex = 5;
            currentPos.Image = await StorageFile.GetFileFromApplicationUriAsync(person);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                map.MapElements.Add(currentPos);
            });

            var a = await Geolocator.RequestAccessAsync();
            if (a == GeolocationAccessStatus.Allowed)
            {
                if (geolocator.LocationStatus == PositionStatus.Ready)
                {
                    Geoposition d = await geolocator.GetGeopositionAsync();
                    Geopoint pos = new Geopoint(d.Coordinate.Point.Position);
                    currentPos.Location = pos;
                    await map.TrySetViewAsync(pos, 17);
                }
            }
        }

        public async void setupGeofencing()
        {
            GeofenceMonitor.Current.Geofences.Clear();
            if (await Geolocator.RequestAccessAsync() == GeolocationAccessStatus.Allowed)
            {
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
        }

        private async void GeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            var reports = sender.ReadReports();

            foreach (GeofenceStateChangeReport report in reports)
            {

                switch (report.NewState)
                {
                    case GeofenceState.None:
                        break;
                    case GeofenceState.Entered:
                        if (report.Geofence.Id != "currentLoc")
                        {
                            string desc = "";
                            foreach (Waypoint e in selectedRoute.waypoints)
                                if (report.Geofence.Id == e.name)
                                {
                                    desc = e.name;
                                    pushNot("Waypoint Nearby", desc);
                                    if (report.Geofence.Id == "Einde stadswandeling")
                                    {
                                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                        {
                                            map.MapElements.Clear();
                                            walkedRoute = new List<Geopoint>();
                                            map.MapElements.Add(currentPos);

                                            //get vvv waypoint
                                            Waypoint vvv = null;
                                            foreach (Waypoint wp in this.selectedRoute.waypoints)
                                            {
                                                if (wp.name == "VVV")
                                                {
                                                    vvv = wp;
                                                }
                                            }

                                            List<Geopoint> waypointList = new List<Geopoint>();
                                            waypointList.Add(currentPos.Location);
                                            waypointList.Add(vvv.GeoPosition());

                                            getMap(waypointList);
                                            addMapIcon(vvv);

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
                                        if (icon.Title == report.Geofence.Id)
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
            if (await Geolocator.RequestAccessAsync() == GeolocationAccessStatus.Allowed)
            {
                try {
                    Geoposition updatedPosition = await geolocator.GetGeopositionAsync();
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        currentPos.Location = updatedPosition.Coordinate.Point;
                    });
                    walkedRoute.Add(updatedPosition.Coordinate.Point);
                    drawWalkedRoute();
                    await map.TrySetViewAsync(updatedPosition.Coordinate.Point, 17);
                }
                catch(System.UnauthorizedAccessException e)
                {
                    //pushNot("GPS Status", "GPS Disabled while retrieving location, Please enable GPS");
                }
            }
        }

        public async void drawWalkedRoute()
        {
            if (walkedRoute.Count >= 2 && dataHandler != null)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MapPolyline updatedWalkedLine = new MapPolyline
                    {
                        StrokeThickness = 11,
                        StrokeColor = Colors.Gray,
                        StrokeDashed = false,
                        ZIndex = 3
                    };

                    List<BasicGeoposition> walkedPointList = new List<BasicGeoposition>();

                    foreach (Geopoint walkedPoint in walkedRoute)
                        walkedPointList.Add(walkedPoint.Position);

                    updatedWalkedLine.Path = new Geopath(walkedPointList);
                    if (walkedLine != null)
                        map.MapElements.Remove(walkedLine);

                    walkedLine = updatedWalkedLine;
                    map.MapElements.Add(walkedLine);
                });
            }
        }

        private void map_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            string description = "No description";
            if (args.MapElements.First() is MapIcon)
            {
                MapIcon mapIcon = (MapIcon)args.MapElements.First();
                description = mapIcon.Title;
            }

            if (description != "Current position")
                foreach (Waypoint wayPoint in selectedRoute.waypoints)
                    if (wayPoint.name == description)
                    {
                        innerFrame.Navigate(typeof(InfoPage), new Tuple<Waypoint, Frame>(wayPoint, innerFrame));
                        isInfoPageOpen = true;
                    }
        }

        public void buildMap()
        {
            List<Geopoint> landMarkList = new List<Geopoint>();
            foreach (Waypoint waypoint in selectedRoute.waypoints)
            {
                landMarkList.Add(waypoint.GeoPosition());
                if (waypoint.landmark)
                {
                    addMapIcon(waypoint);
                    setupGeofences(waypoint);
                }
            }
            getMap(landMarkList);
            isMapBuild = true;
        }

        private async void getMap(List<Geopoint> landMarkList)
        {
            MapRouteFinderResult routeResult = await MapRouteFinder.GetWalkingRouteFromWaypointsAsync(landMarkList);
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
                getMap(landMarkList);
                return;
            }
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                map.MapElements.Add(line);
            });
        }

        private async void addMapIcon(Waypoint waypoint)
        {
            MapIcon mapIcon = new MapIcon();
            mapIcon.Location = waypoint.GeoPosition();
            mapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapIcon.Title = waypoint.name;
            mapIcon.ZIndex = 4;
            mapIcon.Image = await StorageFile.GetFileFromApplicationUriAsync(uri1);

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                map.MapElements.Add(mapIcon);
            });
        }

        private void setupGeofences(Waypoint wayPoint)
        {
            Geofence fence = new Geofence(
                wayPoint.name,
                new Geocircle(wayPoint.GeoPosition().Position, 40),
                MonitoredGeofenceStates.Entered | MonitoredGeofenceStates.Exited,
                false,
                new TimeSpan(0));
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
