using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Pemandangan.Model
{
    class Place
    {
        private bool isPassed;
        private Geocoordinate location;
        private PlaceInfo placeInfo;

        public Place(Geocoordinate location, PlaceInfo placeInfo)
        {
            this.location = location;
            this.placeInfo = placeInfo;
            this.isPassed = false;
        }

        public Geocoordinate ShowPlace()
        {
            return location;
        }
    }
}
