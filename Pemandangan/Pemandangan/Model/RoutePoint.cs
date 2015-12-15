using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Pemandangan.Model
{

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

        private Geocoordinate geoCoordinate;
        private Direction direction;

        public RoutePoint(Geocoordinate geoCoordinate, Direction direction)
        {
            this.geoCoordinate = geoCoordinate;
            this.direction = direction;
        }
    }
}
