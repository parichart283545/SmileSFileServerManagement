using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using S3Upload_Demo2;

namespace SmileDocumentBackupAndDelete
{
    public partial class Service1 : ServiceBase
    {
        public Service1(string mode)
        {
            InitializeComponent();
            if (mode != "" && mode != null)
            {
                MainS3BackupAndDelete.ModeStr = mode;
                MainS3BackupAndDelete.DoInit();
            }
        }

        protected override void OnStart(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                MainS3BackupAndDelete.ModeStr = args[0];
                MainS3BackupAndDelete.DoInit();
            }
        }

        protected override void OnStop()
        {
        }
    }
}