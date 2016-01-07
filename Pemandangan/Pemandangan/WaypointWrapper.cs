using Pemandangan.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Pemandangan
{
    class WaypointWrapper
    {
        public Frame frame { get; }
        public Waypoint w { get; }

        public WaypointWrapper(Waypoint w, Frame frame)
        {
            this.w = w;
            this.frame = frame;
        }
    }
}
