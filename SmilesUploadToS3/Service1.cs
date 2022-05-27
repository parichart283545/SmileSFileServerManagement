using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Demo_AWSS3;
using System.Reflection;
using Amazon;
using Schedule;
using System.Configuration;
using System.IO;
using SmilesUploadToS3.Model;
using System.Threading;
using System.Globalization;
using System.Net;

namespace SmilesUploadToS3
{
    public partial class Service1 : ServiceBase
    {
        public Service1(string mode)
        {
            InitializeComponent();
            if (mode != "" && mode != null)
                S3Main.DoStart(mode);
        }

        protected override void OnStart(string[] args)
        {
            if (args != null && args.Length > 0)
                S3Main.DoStart(args[0]);
        }

        protected override void OnStop()
        {
            S3Main.DoDestroy();
        }
    }
}