using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Pemandangan.Model
{
    [DataContract]
    class PlaceInfo
    {
        [DataMember]
        private string name;
        [DataMember]
        private string information;
        [DataMember]
        private string image, audio;

        //Frits jouw taak 
    }
}
