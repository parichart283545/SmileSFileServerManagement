using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SmileDocumentBackupAndDelete
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            //new Service1("smiledoc");
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1((args != null && args.Length > 0)? args[0]:null/*"smiletest"*/)
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}