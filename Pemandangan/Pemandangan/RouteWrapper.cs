using Pemandangan.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Pemandangan
{
    class RouteWrapper
    {
        public Frame frame { get;}
        private Route route { get;}

        public RouteWrapper(Frame frame, Route route)
        {
            this.frame = frame;
            this.route = route;
        }

    }
}
