using Passengers.Order.CBR.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Order.CBR.SimilarityFunctions
{
    public class SimilarityFunction
    {
        public SimilarityFunction()
        {
            #region Days
            foreach (var row in Enum.GetValues<DayOfWeek>())
            {
                foreach (var col in Enum.GetValues<DayOfWeek>())
                {
                    if (row.Equals(col)
                    || ((row >= DayOfWeek.Sunday && row <= DayOfWeek.Thursday)
                        && (col >= DayOfWeek.Sunday && col <= DayOfWeek.Thursday)))
                    {
                        TableDays.Add((row, col), 1.0);
                    }
                    else if ((row == DayOfWeek.Friday || row == DayOfWeek.Saturday)
                        && (col == DayOfWeek.Friday || col == DayOfWeek.Saturday))
                    {
                        TableDays.Add((row, col), 1.0);
                    }
                    else
                    {
                        TableDays.Add((row, col), 0);
                    }
                }
            }
            #endregion

            #region WeatherForecast
            TableWeatherForcast.Add((WeatherForecast.Sunny, WeatherForecast.Sunny), 1.0);
            TableWeatherForcast.Add((WeatherForecast.Sunny, WeatherForecast.Raining), 5.0);
            TableWeatherForcast.Add((WeatherForecast.Sunny, WeatherForecast.Snowy), 0.0);

            TableWeatherForcast.Add((WeatherForecast.Raining, WeatherForecast.Sunny), 5.0);
            TableWeatherForcast.Add((WeatherForecast.Raining, WeatherForecast.Raining), 1.0);
            TableWeatherForcast.Add((WeatherForecast.Raining, WeatherForecast.Snowy), 5.0);

            TableWeatherForcast.Add((WeatherForecast.Snowy, WeatherForecast.Sunny), 0.0);
            TableWeatherForcast.Add((WeatherForecast.Snowy, WeatherForecast.Raining), 5.0);
            TableWeatherForcast.Add((WeatherForecast.Snowy, WeatherForecast.Snowy), 1.0);
            #endregion

            #region Vehicles
            TableVehicles.Add((VehicleTypes.Bike, VehicleTypes.Bike), 1.0);
            TableVehicles.Add((VehicleTypes.Bike, VehicleTypes.ElectricBike), 0.0);

            TableVehicles.Add((VehicleTypes.ElectricBike, VehicleTypes.Bike), 0.0);
            TableVehicles.Add((VehicleTypes.ElectricBike, VehicleTypes.ElectricBike), 1.0);
            #endregion
        }

        public List<Tuple<double, OrderCBR>> GetSimilarity(List<OrderCBR> orders, OrderCBR newOrder)
        {
            List<Tuple<double, OrderCBR>> tuples = new List<Tuple<double, OrderCBR>>();
            foreach (var order in orders)
            {
                double sum = 0;

                sum += 10.0 * GetTableDays((order.OrderDay, newOrder.OrderDay));
                sum += 6.0 * GetTableWeatherForcast((order.WeatherForcast, newOrder.WeatherForcast));
                sum += 2.0 * GetTableVehicles((order.VehicleType, newOrder.VehicleType));
                sum += 1.0 * GetLinearOrderCount(order.ProductCount, newOrder.ProductCount);
                sum += 5.0 * GetLinearOrderTime(order.PreprationTime, newOrder.PreprationTime);
                sum += 4.0 * GetLegthOfWay(order.LengthOfWay, newOrder.LengthOfWay);

                tuples.Add(new Tuple<double, OrderCBR>(sum, order));
            }

            return tuples.OrderByDescending(t => t.Item1).ToList();
        }

        #region Tables 
        public Dictionary<(DayOfWeek, DayOfWeek), double> TableDays { get; set; } = new Dictionary<(DayOfWeek, DayOfWeek), double>();
        public Dictionary<(WeatherForecast, WeatherForecast), double> TableWeatherForcast { get; set; } = new Dictionary<(WeatherForecast, WeatherForecast), double>();
        public Dictionary<(VehicleTypes, VehicleTypes), double> TableVehicles { get; set; } = new Dictionary<(VehicleTypes, VehicleTypes), double>();

        public double GetTableDays((DayOfWeek, DayOfWeek) key)
        {
            return TableDays[key];
        }

        public double GetTableWeatherForcast((WeatherForecast, WeatherForecast) key)
        {
            return TableWeatherForcast[key];
        }

        public double GetTableVehicles((VehicleTypes, VehicleTypes) key)
        {
            return TableVehicles[key];
        }

        #endregion

        #region Linear

        public double GetLinearOrderCount(int val, int newval)
        {
            return GetLinearVal(1, 10 , val , newval);
        }

        public double GetLinearOrderTime(int val, int newval)
        {
            return GetLinearVal(9, 24, val, newval);
        }

        public double GetLegthOfWay(int val, int newval)
        {
            return GetLinearVal(900, 2000, val, newval);
        }

        public double GetLinearVal(int min , int max , int val, int newval)
        {
            int range = max - min + 1;
            return Math.Round((1.0 / range) * (range - Math.Abs(val - newval)), 2);  
        }

        #endregion


    }
}
