using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace Coordinator
{
    class Control
    {
        public string OutputPY = "";


        public string Fly_UAV(string typee, string IpAddress, string Portt, string CommName)
        {
            CommunicationLinks CommLinksObj = new CommunicationLinks();
            
            var thread = new Thread(new ThreadStart(() =>
            {
                string python = @"C:\Python27\python.exe";
                string myPythonApp = "script-arm-takeoff-and-auto.py";
                string con = typee;
                string ip = IpAddress;
                string port = Portt;
                string arg = "";

                arg = myPythonApp + " " + con + " " + ip + " " + port;   //Final String that will passed to Dronekit

                int indexx = Array.FindIndex(CommLinksObj.UAVinfo, s => s.N_UAV == CommName);
                CommLinksObj.UAVinfo[indexx].UAVAutomataEstate = true;

                ProcessStartInfo psi = new ProcessStartInfo(python, arg);
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.CreateNoWindow = true;

                var proc = Process.Start(psi);

                StreamReader sr = proc.StandardOutput;

                while (!proc.HasExited)
                {
                    if (!sr.EndOfStream)
                    {
                        string procOutput = sr.ReadToEnd();
                        //this.Invoke(new Action<string>(s => { rtbScript.Text += s; }), procOutput);
                        OutputPY = procOutput;

                    }
                    else Thread.Sleep(20);
                }

               
            }));

            thread.Start();

            return OutputPY;

        }

        public void Pause_Mission()
        {

        }














    }
}
