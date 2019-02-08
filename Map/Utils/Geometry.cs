using System;
using GMap.NET;

namespace CoordinatorMap
{
    public static partial class Utils
    {
        /*
         Return the point that belongs to the line segment between points p1 and p2 such that the distance
         between p2 and this point is equal to the distance between p2 and another point ('current')
         */
        public static PointLatLng DistanceProjection(PointLatLng p1, PointLatLng current, PointLatLng p2)
        {
            double D = Math.Sqrt(Math.Pow(p2.Lng - p1.Lng, 2) + Math.Pow(p2.Lat - p1.Lat, 2));
            double d_p2 = Math.Sqrt(Math.Pow(p2.Lng - current.Lng, 2) + Math.Pow(p2.Lat - current.Lat, 2));
            double d_p1 = Math.Sqrt(Math.Pow(current.Lng - p1.Lng, 2) + Math.Pow(current.Lat - p1.Lat, 2));

            if (d_p2 >= D) return p1;
            else if (D <= d_p1) return p2;

            return DistanceProjection(p1, D-d_p2, p2);
        }

        public static PointLatLng DistanceProjection(PointLatLng p1, double d_p1, PointLatLng p2)
        {
            if (p1.Lng == p2.Lng)
                return new PointLatLng(p2.Lat > p1.Lat ? p1.Lat + d_p1 : p1.Lat - d_p1, p2.Lng);

            double b = (p2.Lat - p1.Lat) / (p2.Lng - p1.Lng);
            double c = p1.Lat - b * p1.Lng;
            
            double lng = d_p1 / Math.Sqrt(1 + Math.Pow(b, 2));
            if (p1.Lng > p2.Lng) lng *= -1;
            lng += p1.Lng;

            double lat = b * lng + c;
            
            return new PointLatLng(lat, lng);
        }


        public static double MovementAngularVariation(PointLatLng a, PointLatLng b, double theta)
        {
            theta *= (Math.PI / 180); // Convertion of degrees into radian measure

            double y = a.Lat, x = a.Lng;
            double yf = b.Lat, xf = b.Lng;
            double deltaY = yf - y, deltaX = xf - x;

            double thetaf;
            if (deltaX == 0)
            {
                thetaf = yf > y ? Math.PI / 2 : 3 * Math.PI / 2;
            }
            else if (yf == y)
            {
                if (xf > x)
                    thetaf = 0;
                else
                    thetaf = Math.PI;
            }
            else
            {
                thetaf = Math.Atan(Math.Abs(deltaY / deltaX));

                if (deltaX > 0 && deltaY < 0)
                    thetaf = 2 * Math.PI - thetaf;
                else if (deltaX < 0 && deltaY > 0)
                    thetaf = Math.PI - thetaf;
                else if (deltaX < 0 && deltaY < 0)
                    thetaf += Math.PI;
            }

            double deltaTheta = thetaf - theta;

            if (Math.Abs(deltaTheta) > Math.PI)
            {
                double temp = 2 * Math.PI - Math.Abs(deltaTheta);
                if (deltaTheta < 0)
                    deltaTheta = temp;
                else
                    deltaTheta = -temp;
            }

            return (180 / Math.PI) * deltaTheta;
        }
    }
}
