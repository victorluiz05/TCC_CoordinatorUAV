using System;
using GMap.NET;
using System.Device.Location;

namespace CoordinatorMap
{
    public static partial class Utils
    {
        public static PointLatLng NewLLPoint(PointLatLng old, double dx, double dy, double lat = 91)
        {
            if (lat == 91) lat = old.Lat;

            double newLat = old.Lat + (180 / Math.PI) * (dy / 6378137);
            double newLng = old.Lng + (180 / Math.PI) * (dx / 6378137) / Math.Cos(Math.PI * lat / 180.0);

            return new PointLatLng(newLat, newLng);
        }

        public static double Distance(PointLatLng p1, PointLatLng p2)
        {
            GeoCoordinate c1 = new GeoCoordinate(p1.Lat, p1.Lng);
            GeoCoordinate c2 = new GeoCoordinate(p2.Lat, p2.Lng);
            return c1.GetDistanceTo(c2);
        }
    }
}