using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pemandangan.Model
{
    class Route
    {
        private string name;
        private List<Place> places { get; set; }
        private List<RoutePoint> routePoints { get; set; }

        public Route()
        {
            places = new List<Place>();
            routePoints = new List<RoutePoint>();
        }


    }
}
