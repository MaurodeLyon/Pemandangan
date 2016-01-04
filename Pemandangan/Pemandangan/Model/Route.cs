using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Pemandangan.Model
{
    [DataContract]
    public class Route
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string landmarks { get; set; }
        [DataMember]
        public List<Waypoint> waypoints { get; set; }

        [DataMember]
        private List<Place> places { get; set; }
        [DataMember]
        private List<RoutePoint> routePoints { get; set; }

        public Route()
        {
            places = new List<Place>();
            routePoints = new List<RoutePoint>();
        }
    }
}
