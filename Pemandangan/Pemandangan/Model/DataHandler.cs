using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pemandangan.Model
{
    class DataHandler
    {
        private List<Route> routes { get; set; }
        private List<Language> languages { get; set; }

        public DataHandler()
        {
            this.routes = new List<Route>();
            this.languages = new List<Language>();
            LoadData();
        }

        private void LoadData()
        {
                          
        }

    }
}
