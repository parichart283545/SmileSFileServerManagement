using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SmilesUploadToS3
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            //#if DEBUG
            //Service1 serviced = new Service1("test");
            //serviced.OnDebug();

            //#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1((args != null && args.Length > 0)? args[0]:null/*"sssdoc"*/)
            };
            ServiceBase.Run(ServicesToRun);
            Console.ReadLine();         //#endif
        }
    }
}