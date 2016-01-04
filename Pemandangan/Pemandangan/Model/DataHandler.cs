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
        private List<Language> languages { get; set; }

        public DataHandler()
        {
            languages = new List<Language>();
            //LoadRoutes();
        }

        public async void LoadRoutes()
        {
            string jsonString = "";
            try
            {
                var uri = new System.Uri("ms-appx:///Assets/route_data.json");
                var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
                jsonString = await Windows.Storage.FileIO.ReadTextAsync(file);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("no data: " + e);
            }
            if (!string.IsNullOrEmpty(jsonString))
                lastRoute = JsonConvert.DeserializeObject<Route>(jsonString);
        }

        public Route lastRoute { get; set; }
    }
}
