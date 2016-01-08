using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Pemandangan.Model
{
    [DataContract]
    class RoutePoint
    {
        [DataMember]
        public double longitude { get; set; }
        [DataMember]
        public double latitiude { get; set; }

        public RoutePoint(double longitude, double latitiude)
        {
            this.latitiude = latitiude;
            this.longitude = longitude;
        }

        public Geopoint ShowPlace()
        {
            return new Geopoint(new BasicGeoposition()
            {
                Longitude = longitude,
                Latitude = latitiude
            });
        }
    }
}
