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
        public DataHandler datahandler { get;}

        public RouteWrapper(Frame frame, DataHandler datahandler)
        {
            this.frame = frame;
            this.datahandler = datahandler;
        }

    }
}
