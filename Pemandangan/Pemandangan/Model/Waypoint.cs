using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Pemandangan.Model
{
    public class Waypoint
    {
        public string name { get; set; }
        public Boolean landmark { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }


        public Geopoint GeoPosition()
        {
            return new Geopoint(new BasicGeoposition()
            {
                Longitude = longitude,
                Latitude = latitude
            });
        }
    }
}
