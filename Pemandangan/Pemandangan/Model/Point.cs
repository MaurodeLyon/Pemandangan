using System.Runtime.Serialization;

namespace Pemandangan.Model
{
    [DataContract]
    public class Point
    {
        [DataMember]
        public double longitude { get; set; }
        [DataMember]
        public double latitiude { get; set; }

        public Point(double longitude, double latitiude)
        {
            this.longitude = longitude;
            this.latitiude = latitiude;
        }
    }
}
