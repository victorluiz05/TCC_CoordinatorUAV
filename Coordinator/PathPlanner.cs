using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Globalization;

namespace Coordinator
{
    class PathPlanner
    {
        double BaseLat = -35.363261;
        double BaseLon = 149.165236;
        double BaseAlt = 584.000000;
        int cont = 1;
        int a = 0;

        public void PathPlannigAlgorithm(double lat, double lon)
        {
            string missioname = "m" + cont.ToString();
            string path = @"Missions\" + missioname + ".txt";

            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("QGC WPL 110");
                    try
                    {
                        sw.WriteLine("0\t1\t0\t16\t0\t0\t0\t0\t" +
                                     double.Parse(BaseLat.ToString()).ToString("0.000000", new CultureInfo("en-US")) +
                                     "\t" +
                                     double.Parse(BaseLon.ToString()).ToString("0.000000", new CultureInfo("en-US")) +
                                     "\t" +
                                     double.Parse(BaseAlt.ToString()).ToString("0.000000", new CultureInfo("en-US")) +
                                     "\t1");
                    }
                    catch { } 
                    
                    sw.Write((a + 1)); // seq
                    sw.Write("\t" + 0); // current
                    sw.Write("\t" + "3"); //frame 
                    sw.Write("\t" + "16");
                    sw.Write("\t" +
                             double.Parse("0")
                                         .ToString("0.00000000", new CultureInfo("en-US")));
                    sw.Write("\t" +
                             double.Parse("0")
                                 .ToString("0.00000000", new CultureInfo("en-US")));
                    sw.Write("\t" +
                             double.Parse("0")
                                 .ToString("0.00000000", new CultureInfo("en-US")));
                    sw.Write("\t" +
                             double.Parse("0")
                                 .ToString("0.00000000", new CultureInfo("en-US")));
                    sw.Write("\t" +
                             double.Parse(lat.ToString())
                                 .ToString("0.00000000", new CultureInfo("en-US")));
                    sw.Write("\t" +
                             double.Parse(lon.ToString())
                                 .ToString("0.00000000", new CultureInfo("en-US")));
                    sw.Write("\t" +
                             (double.Parse("100")).ToString("0.000000", new CultureInfo("en-US")));
                    sw.Write("\t" + 1);
                    sw.WriteLine("");

                    sw.Close();
                    cont = cont + 1;
                }
               
            }
            
        }


        public void DecisionalAlgorithm()
        {




        }
    }
}
