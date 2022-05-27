using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Demo_AWSS3
{
    internal static class logging
    {
        public static string pathstr
        {
            get;
            set;
        }

        public static string fileName
        {
            get;
            set;
        }

        public static void Writelog(string messagestr)
        {
            try
            {
                messagestr = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + " " + messagestr;
                FileStream objFilestream = new FileStream(string.Format("{0}\\{1}", pathstr, fileName), FileMode.Append, FileAccess.Write);
                StreamWriter objStreamWriter = new StreamWriter((Stream)objFilestream);
                objStreamWriter.WriteLine(messagestr);
                objStreamWriter.Close();
                objFilestream.Close();
            }
            catch (Exception ex)
            {
                //
            }
        }
    }
}