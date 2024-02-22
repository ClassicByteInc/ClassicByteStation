using ClassicByte.Library.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicByte.Application.StationShell
{
    internal partial class Program:Station
    {
        public static void Run() 
        {
            Console.WriteLine("HelloWorld");
            Console.WriteLine(Workspace);
            Functions.SetEnvironmentVariable("wdnmd","dbnm");
            Console.ReadKey();
        }
    }
}
