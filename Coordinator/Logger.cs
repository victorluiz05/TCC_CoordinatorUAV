using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Coordinator
{
    internal class Logger
    {
        public string Name;

        public Logger(string name)
        {
            Name = name;
        }
        
        public void WriteLog(Exception e, string message)
        {
            WriteLog(e + " : " + message);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void WriteLog(string message)
        {
            Directory.CreateDirectory(@"Log");
            File.AppendAllText(@"Log\" + Name + ".log", DateTime.Now + " - " + message);
        }
    }
}
