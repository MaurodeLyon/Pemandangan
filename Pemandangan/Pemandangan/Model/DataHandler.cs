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
    class DataHandler
    {
        private List<Route> routes { get; set; }
        private List<Language> languages { get; set; }

        public DataHandler()
        {
            routes = new List<Route>();
            languages = new List<Language>();
            //LoadRoutes();
        }

        private void LoadRoutes()
        {
            string jsonString = "";
            try
            {
                using (var stream = ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("routes.json").Result)
                {
                    byte[] result = new byte[stream.Length];
                    stream.ReadAsync(result, 0, result.Length);
                    jsonString = Encoding.ASCII.GetString(result);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("not save data yet ");
                //saveShit(); //file does not exist yet.
            }
            if (!string.IsNullOrEmpty(jsonString))
                routes = JsonConvert.DeserializeObject<List<Route>>(jsonString);
        }

    }
}
