﻿using System;

namespace Passengers.SharedKernel.Map;

public static class MapHelpers
{
    public static double CalculateDistance(this Point point1, Point point2)
    {
        if(point1.Lat == null || point1.Lng == null || point2.Lat == null || point2.Lng == null)
            return 0;
        var latitude1 = double.Parse(point1.Lat);
        var longitude1 = double.Parse(point1.Lng);

        var latitude2 = double.Parse(point2.Lat);
        var longitude2 = double.Parse(point2.Lng);

        var d1 = latitude1 * (Math.PI / 180.0);
        var num1 = longitude1 * (Math.PI / 180.0);
        var d2 = latitude2 * (Math.PI / 180.0);
        var num2 = longitude2 * (Math.PI / 180.0) - num1;
        var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                 Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
        return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
    }
}
