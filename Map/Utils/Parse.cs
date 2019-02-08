using System.Globalization;

namespace CoordinatorMap
{
    public static partial class Utils
    {
        public static double ParseDouble(string str)
        {
            return double.Parse(str, CultureInfo.InvariantCulture);
        }

        public static float ParseFloat(string str)
        {
            return float.Parse(str, CultureInfo.InvariantCulture);
        }
    }
}