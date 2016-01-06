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
        public enum Direction
        {
            North,
            NorthEast,
            East,
            SouthEast,
            South,
            SouthWest,
            West,
            NorthWest
        };

        [DataMember]
        public double longitude { get; set; }
        [DataMember]
        public double latitiude { get; set; }
        [DataMember]
        private Direction direction;

        public RoutePoint(double longitude, double latitiude, Direction direction)
        {
            this.latitiude = latitiude;
            this.longitude = longitude;
            this.direction = direction;
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
