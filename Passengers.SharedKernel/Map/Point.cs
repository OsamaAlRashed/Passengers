using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.SharedKernel.Map
{
    public class Point
    {
        public Point(string lat, string lng)
        {
            Lat = lat;
            Lng = lng;
        }
        public string Lat { get; set; }
        public string Lng { get; set; }
    }
}
