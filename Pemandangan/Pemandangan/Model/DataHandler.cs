using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage;

namespace Pemandangan.Model
{
    public class DataHandler
    {
        public Route selectedRoute { get; set; }

        private static string route_data_path = "Assets/route_data.txt";
        public DataHandler()
        {
            //LoadRoutes();
        }

        public void LoadRoutes()
        {
            string jsonString = "";
            try
            {
                jsonString = File.ReadAllText(route_data_path);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("no data: " + e);
            }
            if (!string.IsNullOrEmpty(jsonString))
                selectedRoute = JsonConvert.DeserializeObject<Route>(jsonString);
        }

    }
}
