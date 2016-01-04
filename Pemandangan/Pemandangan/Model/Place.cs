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
    class Place
    {
        private bool isPassed;
        [DataMember]
        public double longitude { get; set; }
        [DataMember]
        public double latitiude { get; set; }
        [DataMember]
        private PlaceInfo placeInfo;

        public Place(double longitude, double latitiude, PlaceInfo placeInfo)
        {
            this.longitude = longitude;
            this.latitiude = latitiude;
            this.placeInfo = placeInfo;
            this.isPassed = false;
        }

        public Geopoint ShowPlace() { 
            return new Geopoint(new BasicGeoposition()
            {
                Longitude = longitude,
                Latitude = latitiude
            });
        }
    }
}
