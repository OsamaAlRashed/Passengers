using Passengers.Order.CBR.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Order.CBR
{
    public class OrderCBR
    {
        public string CustomerName { get; set; }
        public int ProductCount { get; set; }
        public int LengthOfWay { get; set; }
        public int PreprationTime { get; set; }
        public VehicleTypes VehicleType { get; set; }
        public WeatherForecast WeatherForcast { get; set; }
        public DayOfWeek OrderDay { get; set; }
        public int TimeCost { get; set; }
    }
}
