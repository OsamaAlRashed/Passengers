namespace Passengers.SharedKernel.Map;

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
