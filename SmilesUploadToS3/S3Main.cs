using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ServiceProcess;
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
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.Runtime;
using Amazon.S3.Model;
using System.Timers;

namespace SmilesUploadToS3
{
    public static class S3Main
    {
        #region "Declaration"

        private static System.Timers.Timer aTimer;
        private static bool UploadResult = true;

        private static DateTime StartDT;
        private static DateTime EndDT;

        //private int currtimeout = 100;
        private static int totalleft = 0;

        //private logging writelog = null;
        //private AmazonS3_SDK AWSS3 = null;
        private static string AccKey = "";

        private static string SecKey = "";
        private static string BucketName = "";
        private static string SSSDocToS3Path = "";
        private static string SmileDocToS3Path = "";
        private static string fretype = "";
        private static string frevalue = "";
        private static string fretypevalue = "";
        private static string fretimebegin = "";
        private static string fretimeend = "";
        private static string AWSS3Region = null;
        private static int SleepProcess = 1000;

        private static int c_upload_success = 0;
        private static int c_upload_fail = 0;
        private static int c_s3_connect_fail = 0;
        private static int c_notfound = 0;
        private static int c_total = 0;
        private static int c_loss = 0;
        private static S3StorageClass StorageClass = S3StorageClass.Standard;
        private static string pathstr = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private static ReportTimer _Timer = null;
        private static string ftpPath;
        private static string ftpUsername;
        private static string ftpPassword;
        private static bool ftpEnable = true;
        private static bool IsRunning = false;
        private static bool enablewritelog = true;
        public static string ModeStr = "SSSDoc";
        private static int ModeInt = 1;
        private static string ConnectionStr;

        #endregion "Declaration"

        //

        #region "Event"

        public static void DoStart(string modestr)
        {
            ModeStr = modestr;
            ModeInt = DoConvertModeToInt(modestr);
            switch (ModeInt)
            {
                case 2:
                    Writefilelog("Mode = SSSDoc");
                    break;

                case 3:
                    Writefilelog("Mode = SmileDoc");
                    break;

                default:
                    Writefilelog("Mode = Test");
                    break;
            }

            Writefilelog("Service start");
            //Start Timer
            if (getconfig())
            {
                if (ftpEnable)
                {
                    Writefilelog("Backup to FTP Server is enable, Detail: FTP path=" + ftpPath + " | FTP Username=" + ftpUsername + " | " + "FTP Password=" + ftpPassword);
                    if (!DoConnectFTP())
                    { //Write log file
                        Writefilelog("FTP Server login fail");
                        Thread.Sleep(1000);
                        Process.GetCurrentProcess().Kill();
                    }
                    else { Writefilelog("FTP Server login success"); }
                }
                else { Writefilelog("Backup to FTP Server is disable"); }

                //prepare parameter
                double fredoubvalue = 20; //defualt
                try { if (frevalue != "") { fredoubvalue = Convert.ToDouble(frevalue); } }
                catch (Exception) { Writefilelog("Convert frequency fail. Frequency default value is 20"); fredoubvalue = 20; }

                _Timer = new ReportTimer();
                _Timer.Elapsed += new ReportEventHandler(_Timer_Elapsed);
                _Timer.Error += new Schedule.ExceptionEventHandler(Error);
                if (fretype.IndexOf("daily", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    TimeSpan Tsp = TimeSpan.FromMinutes(fredoubvalue);
                    if (fretypevalue.IndexOf("second", StringComparison.OrdinalIgnoreCase) >= 0)
                    { Tsp = TimeSpan.FromSeconds(fredoubvalue); }
                    else if (fretypevalue.IndexOf("minute", StringComparison.OrdinalIgnoreCase) >= 0)
                    { Tsp = TimeSpan.FromMinutes(fredoubvalue); }
                    else if (fretypevalue.IndexOf("hour", StringComparison.OrdinalIgnoreCase) >= 0)
                    { Tsp = TimeSpan.FromHours(fredoubvalue); }

                    _Timer.AddReportEvent(new BlockWrapper(
                                       new SimpleInterval(DateTime.Now, Tsp),
                                       "Daily",
                                       fretimebegin,
                                       fretimeend),
                                       1);
                }
                else
                {
                    if (fretypevalue.IndexOf("second", StringComparison.OrdinalIgnoreCase) >= 0)
                    { _Timer.AddReportEvent(new ScheduledTime("BySecond", fredoubvalue.ToString()), 1); }
                    else if (fretypevalue.IndexOf("minute", StringComparison.OrdinalIgnoreCase) >= 0)
                    { _Timer.AddReportEvent(new ScheduledTime("BySecond", (fredoubvalue * 1000).ToString()), 1); }
                    else if (fretypevalue.IndexOf("hour", StringComparison.OrdinalIgnoreCase) >= 0)
                    { _Timer.AddReportEvent(new ScheduledTime("ByHour", fredoubvalue.ToString()), 1); }
                    else if (fretypevalue.IndexOf("weekly", StringComparison.OrdinalIgnoreCase) >= 0)
                    { _Timer.AddReportEvent(new ScheduledTime("Weekly", fredoubvalue.ToString()), 1); }
                    else if (fretypevalue.IndexOf("once", StringComparison.OrdinalIgnoreCase) >= 0)
                    { _Timer.AddReportEvent(new SingleEvent(DateTime.ParseExact(fretimebegin, "yy/MM/dd h:mm:ss tt", CultureInfo.InvariantCulture)), 1); }
                }
                _Timer.Start();
            }
        }

        public static void DoDestroy()
        {
            try
            {
                _Timer.Stop();
                _Timer.Dispose();
                Writefilelog("Service stop");
                //Thread.Sleep(1000);
                //Process.GetCurrentProcess().Kill();
            }
            catch (Exception)
            {
                //
            }
        }

        private static void Error(object sender, Schedule.ExceptionEventArgs Args)
        {
            Writefilelog("Timer error: " + Args.Error.Message);
        }

        private static void _Timer_Elapsed(object sender, ReportEventArgs e)
        {
            S3UploaderModelDataContext DataContext1 = new S3UploaderModelDataContext(ConnectionStr);
            try
            {
                if (IsRunning != true)
                {
                    Writefilelog("Backup process is beginning");
                    //add timer stop
                    getconfig();
                    //Test database connection

                    if (ftpEnable)
                    {
                        if (!DoConnectFTP())
                        {
                            Writefilelog("FTP Server login fail");
                            //Insert error log
                            DataContext1.usp_Uploader_Log_Insert("", "Error", "FTP Server login fail", "Process");
                            Thread.Sleep(1000);
                            Process.GetCurrentProcess().Kill();
                            return;
                        }
                    }

                    DoRunningStamp(true);

                    switch (ModeInt)
                    {
                        case 2:
                            UploadSSSDocMode();
                            break;

                        case 3:
                            UploadSmileDocMode();
                            break;

                        default:
                            UploadTestMode();
                            break;
                    }

                    //add timer start
                    Writefilelog("Backup process has end");
                    DoRunningStamp(false);
                }
                else
                {
                    Writefilelog("Backup process new round");
                }
            }
            catch (Exception ex)
            {
                Writefilelog("Error thread _Timer_Elapsed: " + ex.Message);
            }
            finally
            {
                DataContext1.Connection.Close();
                DataContext1 = null;
            }
        }

        #endregion "Event"

        #region "Method"

        #region Main Mode

        private static void UploadTestMode()
        {
            S3UploaderModelDataContext DataContext1 = new S3UploaderModelDataContext(ConnectionStr);
            try
            {
                //SSSDoc
                Writefilelog("Start upload SSSDoc(test)");
                Reset_count();//reset counting
                StartDT = DateTime.Now;
                List<usp_DocumentFilelist_SelectResult> lstSSSDoc = new List<usp_DocumentFilelist_SelectResult>(); //Create list result
                lstSSSDoc = DataContext1.usp_DocumentFilelist_Select().ToList();//Define param
                totalleft = lstSSSDoc.Count;
                c_total = totalleft;
                if (totalleft > 0)
                {
                    //put data in each param
                    foreach (var data in lstSSSDoc)
                    {
                        string LocalPath = data.PhysicalPath;
                        string FileNam = data.FileName;

                        //TEST*******
                        //if (!File.Exists(LocalPath))
                        //{
                        //    if (!Directory.Exists(Path.GetDirectoryName(LocalPath)))
                        //    { Directory.CreateDirectory(Path.GetDirectoryName(LocalPath)); }
                        //    File.Copy("E:\\Test Upload\\610000001.pdf", LocalPath);
                        //    //File.Move("E:\\Test Upload 10\\A17_FlightPlan.pdf", LocalPath);
                        //}

                        //for loop get file and upload to S3
                        if (File.Exists(LocalPath))
                        {
                            ThreadUpload("SSSDoc", LocalPath, SSSDocToS3Path + data.SubFolder, FileNam, data.Code);
                        }
                        else //if cannot find file in the location
                        {
                            if (totalleft > 0)
                                totalleft--;
                            c_notfound++;
                            ThreadCannotFind(data.PhysicalPath, data.Code, true);
                        }
                    }
                }
                //Clear cache
                lstSSSDoc.Clear();
                //}
                EndDT = DateTime.Now;
                Write_summary();

                #region SmileDoc

                //Writefilelog("Start upload SmileDoc");
                //Reset_count();//reset counting
                //StartDT = DateTime.Now;
                //List<usp_SmileDoclist_SelectResult> lstSmileDoc = new List<usp_SmileDoclist_SelectResult>(); //Create list result
                //lstSmileDoc = DataContext1.usp_SmileDoclist_Select().ToList();//Define param
                //totalleft = lstSmileDoc.Count;
                //c_total = totalleft;
                //if (totalleft > 0)
                //{
                //    //put data in each param
                //    foreach (var data in lstSmileDoc)
                //    {
                //        string LocalPath = data.PhysicalPath;
                //        string FileNam = data.FileName;
                //        //for loop get file and upload to S3
                //        if (File.Exists(LocalPath))
                //        {
                //            ThreadUpload("SmileDoc", LocalPath, SmileDocToS3Path + data.SubFolder, FileNam, data.AttachmentID.ToString());
                //        }
                //        else //if cannot find file in the location
                //        {
                //            if (totalleft > 0)
                //                totalleft--;
                //            c_notfound++;
                //            ThreadCannotFind(data.PhysicalPath, data.AttachmentID.ToString(), false);
                //        }
                //    }
                //}
                ////Clear cache
                //lstSmileDoc.Clear();
                ////}
                //EndDT = DateTime.Now;
                //Write_summary();

                #endregion SmileDoc
            }
            catch (Exception ex)
            {
                Writefilelog("Error thread UploadTestMode: " + ex.Message);
            }
            finally
            {
                DataContext1.Connection.Close();
                DataContext1 = null;
            }
        }

        private static void UploadSSSDocMode()
        {
            S3UploaderModelDataContext DataContext1 = new S3UploaderModelDataContext(ConnectionStr);
            try
            {
                Writefilelog("Start upload SSSDoc");
                Reset_count();//reset counting
                StartDT = DateTime.Now;
                List<usp_DocumentFilelist_SelectResult> lstSSSDoc = new List<usp_DocumentFilelist_SelectResult>(); //Create list result
                lstSSSDoc = DataContext1.usp_DocumentFilelist_Select().ToList();//Define param
                totalleft = lstSSSDoc.Count;
                c_total = totalleft;
                if (totalleft > 0)
                {
                    //put data in each param
                    foreach (var data in lstSSSDoc)
                    {
                        string LocalPath = data.PhysicalPath;
                        string FileNam = data.FileName;
                        //for loop get file and upload to S3
                        if (File.Exists(LocalPath))
                        {
                            ThreadUpload("SSSDoc", LocalPath, SSSDocToS3Path + data.SubFolder, FileNam, data.Code);
                            //await Task.Run(() => ThreadUpload("SSSDoc", LocalPath, SSSDocToS3Path + data.SubFolder, FileNam, data.Code));
                            //Thread.Sleep(SleepProcess);
                        }
                        else //if cannot find file in the location
                        {
                            if (totalleft > 0)
                                totalleft--;
                            c_notfound++;
                            ThreadCannotFind(data.PhysicalPath, data.Code, true);
                            //await Task.Run(() => ThreadCannotFind(data.PhysicalPath, data.Code, true));
                            //Thread.Sleep(10);
                        }
                    }
                }

                //Clear cache
                lstSSSDoc.Clear();
                //}
                EndDT = DateTime.Now;
                //Thread.Sleep(5000);
                Write_summary();
            }
            catch (Exception ex)
            {
                Writefilelog("Error thread UploadSSSDocMode: " + ex.Message);
            }
            finally
            {
                DataContext1.Connection.Close();
                DataContext1 = null;
            }
        }

        private static void UploadSmileDocMode()
        {
            S3UploaderModelDataContext DataContext1 = new S3UploaderModelDataContext(ConnectionStr);
            try
            {
                Writefilelog("Start upload SmileDoc");
                Reset_count();//reset counting
                StartDT = DateTime.Now;
                List<usp_SmileDoclist_SelectResult> lst = new List<usp_SmileDoclist_SelectResult>(); //Create list result
                lst = DataContext1.usp_SmileDoclist_Select().ToList();//Define param
                totalleft = lst.Count;
                c_total = totalleft;
                if (totalleft > 0)
                {
                    //put data in each param
                    foreach (var data in lst)
                    {
                        string LocalPath = data.PhysicalPath;
                        string FileNam = data.FileName;
                        //for loop get file and upload to S3
                        if (File.Exists(LocalPath))
                        {
                            ThreadUpload("SmileDoc", LocalPath, SmileDocToS3Path + data.SubFolder, FileNam, data.AttachmentID.ToString());
                            //await Task.Run(() => ThreadUpload("SmileDoc", LocalPath, SmileDocToS3Path + data.SubFolder, FileNam, data.AttachmentID.ToString()));
                            //Thread.Sleep(SleepProcess);
                        }
                        else //if cannot find file in the location
                        {
                            if (totalleft > 0)
                                totalleft--;
                            c_notfound++;
                            ThreadCannotFind(data.PhysicalPath, data.AttachmentID.ToString(), false);
                            //await Task.Run(() => ThreadCannotFind(data.PhysicalPath, data.AttachmentID.ToString(), false));
                            //Thread.Sleep(10);
                        }
                    }
                }

                //Clear cache
                lst.Clear();
                //}
                EndDT = DateTime.Now;
                //Thread.Sleep(5000);
                Write_summary();
            }
            catch (Exception ex)
            {
                Writefilelog("Error thread UploadSmileDocMode: " + ex.Message);
            }
            finally
            {
                DataContext1.Connection.Close();
                DataContext1 = null;
            }
        }

        #endregion Main Mode

        private static string DoUpdateDBUpload(bool IsSucc, string doctype, string s3path, string s3filename, string doccode, string errmsg)
        {
            S3UploaderModelDataContext DataContext = new S3UploaderModelDataContext(ConnectionStr);
            try
            {
                if (IsSucc) //upload success
                {
                    if (doctype == "SSSDoc")
                        DataContext.usp_DocumentFilelistS3_Update(doccode, true, BucketName, AWSS3Region, s3path + s3filename);
                    else DataContext.usp_SmileDoclistS3_Update(doccode, true, BucketName, AWSS3Region, s3path + s3filename);
                }
                else
                {
                    //Insert error log
                    DataContext.usp_Uploader_Log_Insert(doccode, s3filename, "Upload to S3 fail : " + errmsg, doctype);
                    if (doctype == "SSSDoc")
                        DataContext.usp_DocumentFilelistS3_Update(doccode, true, null, null, null);
                    else DataContext.usp_SmileDoclistS3_Update(doccode, true, null, null, null);
                }
                return "success";
            }
            catch (Exception ex)
            {
                return "Update database error : " + ex.Message;
            }
        }

        private static string UploadAsync(string localpath, string s3path)
        {
            try
            {
                if (localpath.Trim() == string.Empty)
                { return "no file name"; }

                AmazonS3Config config = new AmazonS3Config();
                config.RegionEndpoint = Amazon.RegionEndpoint.APSoutheast1;

                AmazonS3Client s3Client = new AmazonS3Client(AccKey,
                                              SecKey,
                                              config);

                s3Client.AfterResponseEvent += _afterEvent;
                s3Client.BeforeRequestEvent += _BeforeEvent;
                s3Client.ExceptionEvent += _OnEvent;
                //Writefilelog("Start upload: " + localpath);
                UploadResult = true;
                TrackMPUAsync(s3Client, localpath, s3path).Wait();
                //Writefilelog("End upload: " + localpath);
                if (UploadResult)
                    return "";
                else
                    return "fail";
            }
            catch (AmazonS3Exception e)
            {
                string err = string.Format("Error encountered on server. Message:'{0}' when writing an object", e.Message);
                Writefilelog(err);
                return err;
            }
            catch (Exception e)
            {
                string err = string.Format("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
                Writefilelog(err);
                return err;
            }
        }

        private static void _OnEvent(object sender, Amazon.Runtime.ExceptionEventArgs e)
        {
            WebServiceExceptionEventArgs tu = (WebServiceExceptionEventArgs)e;
            Writefilelog("S3 error event : " + tu.Exception.Message);
        }

        private static async Task TrackMPUAsync(AmazonS3Client s3cli, string localpath, string s3path)
        {
            try
            {
                var fileTransferUtility = new TransferUtility(s3cli);

                // Use TransferUtilityUploadRequest to configure options.
                // In this example we subscribe to an event.
                var uploadRequest =
                    new TransferUtilityUploadRequest
                    {
                        BucketName = BucketName,
                        StorageClass = StorageClass,
                        FilePath = localpath,
                        Key = s3path
                    };

                uploadRequest.UploadProgressEvent +=
                    new EventHandler<UploadProgressArgs>
                        (uploadRequest_UploadPartProgressEvent);

                await fileTransferUtility.UploadAsync(uploadRequest);
                //Console.WriteLine("Upload completed");
            }
            catch (AmazonS3Exception e)
            {
                Writefilelog(string.Format("Error encountered on server. Message:'{0}' when writing an object", e.Message));
            }
            catch (Exception e)
            {
                Writefilelog(string.Format("Unknown encountered on server. Message:'{0}' when writing an object", e.Message));
            }
        }

        private static void uploadRequest_UploadPartProgressEvent(object sender, UploadProgressArgs e)
        {
            // Process event.
            //Console.WriteLine("{0}/{1}", e.TransferredBytes, e.TotalBytes);
        }

        private static void _BeforeEvent(object sender, RequestEventArgs e)
        {
            //WebServiceRequestEventArgs u = (WebServiceRequestEventArgs)e;
            //AmazonWebServiceRequest serre = u.Request;
            //if (serre.GetType().Name == "InitiateMultipartUploadRequest")
            //{
            //    InitiateMultipartUploadRequest req = (InitiateMultipartUploadRequest)u.Request;
            //    writelog("Before upload detail(InitiateMultipartUploadRequest) :" + req.Key);
            //}
            //else if (serre.GetType().Name == "UploadPartRequest")
            //{
            //    UploadPartRequest upre = (UploadPartRequest)u.Request;
            //    writelog("Before upload detail(UploadPartRequest) :" + upre.Key);
            //}
            //else if (serre.GetType().Name == "CompleteMultipartUploadRequest")
            //{
            //    CompleteMultipartUploadRequest comre = (CompleteMultipartUploadRequest)serre;
            //    writelog("Before upload detail(CompleteMultipartUploadRequest) :" + comre.Key);
            //}
        }

        private static void _afterEvent(object sender, ResponseEventArgs e)
        {
            WebServiceResponseEventArgs u = (WebServiceResponseEventArgs)e;
            AmazonWebServiceResponse serres = u.Response;
            AmazonWebServiceRequest serre = u.Request;
            if (serres.GetType().Name == "UploadPartResponse")
            {
                //UploadPartResponse respo = (UploadPartResponse)u.Response;
                //UploadPartRequest upre = (UploadPartRequest)u.Request;
                //Writefilelog("After upload detail(UploadPartResponse) : " + upre.Key + " /size:" + upre.PartSize.ToString() + " result: " + respo.HttpStatusCode.ToString());
            }
            else if (serres.GetType().Name == "InitiateMultipartUploadResponse")
            {
                //InitiateMultipartUploadResponse respo = (InitiateMultipartUploadResponse)u.Response;
                //InitiateMultipartUploadRequest req = (InitiateMultipartUploadRequest)u.Request;
                //Writefilelog("After upload detail(InitiateMultipartUploadResponse) : " + req.Key + " result: " + respo.HttpStatusCode.ToString());
            }
            else if (serres.GetType().Name == "CompleteMultipartUploadResponse")
            {
                CompleteMultipartUploadRequest respo = (CompleteMultipartUploadRequest)serre;
                CompleteMultipartUploadResponse req = (CompleteMultipartUploadResponse)serres;
                Writefilelog("After upload detail(CompleteMultipartUploadResponse) : " + req.Key + " result: " + req.HttpStatusCode.ToString());
                if (req.HttpStatusCode.ToString() == "OK")
                    UploadResult = true;
                else UploadResult = false;
            }
        }

        private static void ThreadUpload(string doctype, string localpath, string s3path, string s3filename, string doccode)
        {
            S3UploaderModelDataContext DataContext = new S3UploaderModelDataContext(ConnectionStr);
            try
            {
                //Upload to FTP
                if (ftpEnable)
                    UploadtoFTP(localpath, s3filename);
                //upload to S3
                //string uploadresult = AWSS3.File_Upload(localpath, BucketName, s3path, s3filename);
                string uploadresult = UploadAsync(localpath, s3path + s3filename);
                if (uploadresult == "")
                {
                    c_upload_success++;
                    totalleft--;
                    //Update store procedure Insert log is success
                    if (doctype == "SSSDoc")
                        DataContext.usp_DocumentFilelistS3_Update(doccode, true, BucketName, StorageClass.Value, s3path + s3filename);
                    else DataContext.usp_SmileDoclistS3_Update(doccode, true, BucketName, StorageClass.Value, s3path + s3filename);
                    //Write file log
                    Writefilelog("success upload at code:" + doccode + " | path:" + localpath);
                }
                else //if cannot upload file to AWS S3
                {
                    c_upload_fail++;
                    totalleft--;
                    //Write file log
                    Writefilelog("fail upload at code:" + doccode + " | path:" + localpath);
                    //Writefilelog(uploadresult);
                    //Insert error log
                    DataContext.usp_Uploader_Log_Insert(doccode, s3filename, "Upload to S3 fail : " + uploadresult, doctype);
                    //Update store procedure Insert log is fail
                    if (doctype == "SSSDoc")
                    {
                        DataContext.usp_DocumentFilelistS3_Update(doccode, true, null, null, null);
                    }
                    else
                    {
                        DataContext.usp_SmileDoclistS3_Update(doccode, true, null, null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Writefilelog("Error thread ThreadUpload: " + ex.Message);
            }
            finally
            {
                DataContext.Connection.Close();
                DataContext = null;
            }
        }

        private static void ThreadCannotFind(string localpath, string doccode, bool IsSSS)
        {
            S3UploaderModelDataContext DataContext = new S3UploaderModelDataContext(ConnectionStr);
            try
            {
                //Update store procedure Insert log is fail
                Writefilelog("file cannot be found code:" + doccode + " | path:" + localpath);
                //Update store procedure Insert log is fail
                if (IsSSS)
                {
                    //Update Document table
                    DataContext.usp_DocumentFilelistS3_Update(doccode, true, null, null, null);
                    //Insert error log
                    DataContext.usp_Uploader_Log_Insert(doccode, localpath, "file cannot be found in local path", "SSSDoc");
                }
                else
                {
                    //Update Document table
                    DataContext.usp_SmileDoclistS3_Update(doccode, true, null, null, null);
                    //Insert error log
                    DataContext.usp_Uploader_Log_Insert(doccode, localpath, "file cannot be found in local path", "SmileDoc");
                }
            }
            catch (Exception ex)
            {
                Writefilelog("Error thread ThreadCannotFind: " + ex.Message);
            }
            finally
            {
                DataContext.Connection.Close();
                DataContext = null;
            }
        }

        private static bool CheckIfFileExistsOnServer(string fileName)
        {
            string noDpath = fileName.Replace("D:\\", "");
            string flipsl = noDpath.Replace("\\", "/");

            var request = (FtpWebRequest)WebRequest.Create(ftpPath + flipsl);
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    return false;
            }
            return false;
        }

        private static void CreateDir(string NewDir)
        {
            WebRequest request = WebRequest.Create(NewDir);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
            }
        }

        private static void UploadtoFTP(string filepath, string filename)
        {
            string[] lst = null;
            try
            {
                //Prepare parameter
                string noDpath = filepath.Replace("D:\\", "");
                string Ftppath = ftpPath + "/" + noDpath.Replace("\\", "/");
                string nofile = noDpath.Replace("\\" + filename, "");
                lst = nofile.Split(new char[] { '\\' });
            }
            catch (Exception ex)
            {
                Writefilelog("Error thread UploadtoFTP: " + ex.Message);
                return;
            }

            //Check file is exist
            if (CheckIfFileExistsOnServer(filepath))
            { return; }
            else
            {
                string pathftp = ftpPath;
                foreach (string data in lst)
                {
                    pathftp += "/" + data;
                    CreateDir(pathftp);
                }

                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                        client.UploadFile(pathftp + "/" + filename, WebRequestMethods.Ftp.UploadFile, filepath);
                        Writefilelog("Upload file to FTP success: Path=" + filepath + " | Filename:" + filename);
                    }
                }
                catch (WebException e)
                {
                    FtpWebResponse response3 = (FtpWebResponse)e.Response;
                    Writefilelog("Upload file to FTP fail: Path=" + filepath + " | Filename:" + filename + " | Error:" + response3.StatusCode);
                }
            }
        }

        private static bool getconfig()
        {
            try
            {
                //Get parameter from database (folloe by mode)
                switch (ModeInt)
                {
                    case 2:
                        ConnectionStr = Properties.Settings.Default.DBSSSDoc;
                        break;

                    case 3:
                        ConnectionStr = Properties.Settings.Default.DBSmileDoc;
                        break;

                    default:
                        ConnectionStr = Properties.Settings.Default.DBTest;
                        break;
                }
                Writefilelog("Connection string =" + ConnectionStr);

                //Get configuration in database
                S3UploaderModelDataContext DataContext = new S3UploaderModelDataContext(ConnectionStr);
                List<usp_UploaderConfig_SelectResult> lst = new List<usp_UploaderConfig_SelectResult>(); //Create list result
                lst = DataContext.usp_UploaderConfig_Select(null).ToList();
                string sclass = "";
                foreach (var data in lst)
                {
                    if (data.ParameterName == "S3Accesskey") { AccKey = data.ValueString; }
                    else if (data.ParameterName == "S3Secretkey") { SecKey = data.ValueString; }
                    else if (data.ParameterName == "S3BucketName") { BucketName = data.ValueString; }
                    else if (data.ParameterName == "S3Frequency") { fretypevalue = data.ValueString; frevalue = (data.ValueNumber).ToString(); }
                    else if (data.ParameterName == "S3FrequencyType") { fretype = data.ValueString; }
                    else if (data.ParameterName == "S3FrequencyBegin") { fretimebegin = data.ValueString; }
                    else if (data.ParameterName == "S3FrequencyEnd") { fretimeend = data.ValueString; }
                    else if (data.ParameterName == "S3RegionName") { AWSS3Region = data.ValueString; }
                    else if (data.ParameterName == "SmileDocKey") { SmileDocToS3Path = data.ValueString; }
                    else if (data.ParameterName == "SSSDocKey") { SSSDocToS3Path = data.ValueString; }
                    else if (data.ParameterName == "FTPPath") { ftpPath = data.ValueString; }
                    else if (data.ParameterName == "FTPUser") { ftpUsername = data.ValueString; }
                    else if (data.ParameterName == "FTPPassword") { ftpPassword = data.ValueString; }
                    else if (data.ParameterName == "FTPEnable") { ftpEnable = (bool)data.ValueBoolean; }
                    else if (data.ParameterName == "ProcessTimeSleep") { SleepProcess = Convert.ToInt16(data.ValueNumber); }
                    else if (data.ParameterName == "EnableLog") { enablewritelog = (bool)data.ValueBoolean; }
                    else if (data.ParameterName == "S3StorageClass") { sclass = data.ValueString; }
                }

                StorageClass = S3StorageClass.FindValue(sclass);
                lst.Clear();
                DataContext.Connection.Close();
                DataContext = null;
                return true;
            }
            catch (Exception ex)
            {   //Set service stop
                Writefilelog("Read configuration error:" + ex.Message);
                if (aTimer != null)
                { aTimer = null; }
                aTimer = new System.Timers.Timer(300000);
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                aTimer.Enabled = true;
                return false;
            }
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            aTimer.Enabled = false;
            Writefilelog("Start Timer");
            DoStart(ModeStr);
        }

        private static void Writefilelog(string messstr)
        {
            logging.pathstr = pathstr;
            logging.fileName = string.Format(ModeStr + "_Log {0}.log", DateTime.Now.ToString("dd-MM-yyyy"));
            logging.Writelog(messstr);
        }

        private static void Reset_count()
        {
            totalleft = 0;
            c_upload_success = 0;
            c_upload_fail = 0;
            c_s3_connect_fail = 0;
            c_notfound = 0;
            c_total = 0;
            c_loss = 0;
        }

        private static void Write_summary()
        {
            c_loss = totalleft;
            TimeSpan diff = EndDT - StartDT;
            Writefilelog("Summary Detail                :Start at " + StartDT.ToString("dd/MM/yyyy HH:mm:ss") + " End at " + EndDT.ToString("dd/MM/yyyy HH:mm:ss"));
            Writefilelog("Time usage                    :" + diff.TotalMinutes.ToString() + " minutes.");
            Writefilelog("Total files                   :" + String.Format("{0:n0}", c_total));
            Writefilelog("Total files upload success    :" + String.Format("{0:n0}", c_upload_success));
            Writefilelog("Total files upload fail       :" + String.Format("{0:n0}", c_upload_fail));
            Writefilelog("Total files upload fail(loss) :" + String.Format("{0:n0}", c_loss));
            Writefilelog("Total files connection fail   :" + String.Format("{0:n0}", c_s3_connect_fail));
            Writefilelog("Total files not found         :" + String.Format("{0:n0}", c_notfound));
        }

        private static bool DoConnectFTP()
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpPath);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                response.Close();
                //Writefilelog("Connect FTP success");
                return true;
            }
            catch (WebException ex)
            {
                Writefilelog("Connect FTP fail detail: " + ex.Message);
                return false;
            }
        }

        private static void DoRunningStamp(bool value)
        {
            IsRunning = value;
            S3UploaderModelDataContext DataContext = new S3UploaderModelDataContext(ConnectionStr);
            try
            {
                DataContext.usp_UploaderProcess_Update(value);
            }
            catch (Exception ex)
            {
                //Message
                Writefilelog("Stamp process error (DoRunningStamp): " + ex.Message);
            }
            finally
            {
                DataContext.Connection.Close();
                DataContext = null;
            }
        }

        private static int DoConvertModeToInt(string M_str)
        {
            int return_value = 1; //default is test mode
            try
            {
                if (M_str.Equals("sssdoc", StringComparison.OrdinalIgnoreCase))
                { return_value = 2; }
                else if (M_str.Equals("smiledoc", StringComparison.OrdinalIgnoreCase))
                { return_value = 3; }
                return return_value;
            }
            catch { return return_value; }
        }

        #endregion "Method"
    }
}