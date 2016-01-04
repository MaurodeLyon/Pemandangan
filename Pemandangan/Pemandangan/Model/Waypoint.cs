using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
