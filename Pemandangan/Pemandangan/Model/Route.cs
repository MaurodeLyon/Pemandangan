using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Pemandangan.Model
{
    [DataContract]
    class Route
    {
        [DataMember]
        private string name;
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
