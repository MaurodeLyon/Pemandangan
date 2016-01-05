﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Pemandangan.Model
{
    class PlaceInfo
    {

        private Dictionary<int, BitmapImage> places = new Dictionary<int, BitmapImage>();

        public PlaceInfo()
        {
            try {
                places.Add(1, new BitmapImage(new Uri("ms-appx: /Pictures/standbeeld.jpg", UriKind.Relative)));
                places.Add(2, new BitmapImage(new Uri("/Pictures/standbeeld.jpg", UriKind.Relative)));
                places.Add(3, new BitmapImage(new Uri("/Pictures/standbeeld.jpg", UriKind.Relative)));
                places.Add(4, new BitmapImage(new Uri("/Pictures/standbeeld.jpg", UriKind.Relative)));
            }
            catch(ArgumentException ex)
            {
                
            }

        }

        public BitmapImage getPlace(int key)
        {
            try {
                return places[key];
            }
            catch(KeyNotFoundException ex)
            {
                return null;
            }
        }

    }
}
