using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage;
using Windows.Devices.Geolocation;

namespace Pemandangan.Model
{
    public class DataHandler
    {
        public Route selectedRoute { get; set; }
        public List<Route> routes;
        public bool routeInProgress { get; set; }
        public List<Point> walkedRoute { get; set; }

        private static string route_data_path = "Assets/route_data.txt";
        public DataHandler()
        {
            walkedRoute = new List<Point>();
            LoadRoutes();
        }

        public void LoadRoutes()
        {
            string jsonString = "";
            try
            {
                jsonString = File.ReadAllText(route_data_path);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("no data: " + e);
            }
            if (!string.IsNullOrEmpty(jsonString))
                routes = JsonConvert.DeserializeObject<List<Route>>(jsonString);
        }

        public List<Geopoint> getWalkedRouteGeoPositions()
        {
            List<Geopoint> geopoints = new List<Geopoint>();
            foreach (Point point in walkedRoute)
                geopoints.Add(new Geopoint(new BasicGeoposition() { Longitude = point.longitude, Latitude = point.latitiude }));
            return geopoints;
        }

    }
}
