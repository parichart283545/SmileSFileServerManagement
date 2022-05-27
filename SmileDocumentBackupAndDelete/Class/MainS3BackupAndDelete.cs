using Amazon.S3;
using SmileDocumentBackupAndDelete.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LoggingN;
using System.Timers;
using System.Net;
using System.IO;
using Amazon.S3.Model;
using Amazon;
using Schedule;
using System.Globalization;
using System.Threading;
using System.Diagnostics;

namespace S3Upload_Demo2
{
    public static class MainS3BackupAndDelete
    {
        #region Declaration

        private static ReportTimer _Timer = null;
        private static string s3AccKey_read;
        private static string s3SecKey_read;
        private static string s3Bucket;
        private static string ftpHost;
        private static string ftpUser;
        private static string ftpPassword;
        public static string ModeStr;
        private static int ModeInt;
        private static string ConnectionStr;
        private static string pathstr = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private static System.Timers.Timer aTimer;
        private static bool isRunning = false;

        private static string fretype = "";
        private static string frevalue = "";
        private static string fretypevalue = "";
        private static string fretimebegin = "";
        private static string fretimeend = "";

        #endregion Declaration

        #region Method

        //

        #region FTP Zone

        private static bool ConnectFTP()
        {
            var ftpClient = new System.Net.FtpClient.FtpClient();
            try
            {
                // initialize client and connect like you normally would

                ftpClient.Port = 21;
                ftpClient.Host = ftpHost.Replace("ftp://", "");
                ftpClient.Credentials = new System.Net.NetworkCredential(ftpUser, ftpPassword);
                ftpClient.Connect();
                if (ftpClient.IsConnected)
                {
                    // disconnect like you normally would
                    ftpClient.Disconnect();
                    ftpClient.Dispose();
                    return true;
                }
                else
                {
                    Writefilelog("Connect FTP fail, please check ip address or username or password.");
                    ftpClient.Dispose();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Writefilelog("Connect FTP error detail: " + ex.Message);
                if (ftpClient != null)
                    ftpClient.Dispose();
                return false;
            }
        }

        private static bool UploadtoFTP(string doccode, string filepath, string filename)
        {
            string noDrive;
            try
            {
                //Prepare parameter
                string dir = Path.GetDirectoryName(filepath);
                noDrive = dir.Replace("D:\\", "");
                noDrive = noDrive.Replace("\\", "/");
            }
            catch (Exception ex)
            {
                Writefilelog("Backup file to FTP fail replace string error : Path=" + filepath + " | Filename:" + filename + " | Error:" + ex.Message);
                //Write log db
                faillogDB(doccode, filename, "replace string error: " + ex.Message, ModeStr);
                return false;
            }
            string pathftp = "";
            pathftp = ftpHost + "/" + noDrive;
            //For test
            if (ModeInt == 1 || ModeInt == 3)
            { pathftp = ftpHost + "/YingTest/" + noDrive; }

            //Production
            //string pathftp = ftpHost+ "/" + noDrive;

            CreateDir(pathftp);

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                    client.UploadFile(pathftp + "/" + filename, WebRequestMethods.Ftp.UploadFile, filepath);
                    Writefilelog(string.Format("Backup success detail: Code={0}, Dir={1}", doccode, filepath));
                    return true;
                }
            }
            catch (WebException e)
            {
                FtpWebResponse response3 = (FtpWebResponse)e.Response;
                Writefilelog("Backup file to FTP fail: Path=" + filepath + " | Filename:" + filename + " | Error:" + response3.StatusCode);
                //Write log db
                faillogDB(doccode, filename, "status code: " + response3.StatusCode + " description: " + response3.StatusDescription, ModeStr);
                return false;
            }
        }

        //private static bool UploadtoFTPFromStream(string doccode, string filepath, Stream s3file)
        //{
        //    string noDrive;
        //    string filename = Path.GetFileName(filepath);
        //    string pathftp = "";
        //    //Prepare parameter
        //    string dir = Path.GetDirectoryName(filepath);

        //    try
        //    {
        //        noDrive = dir.Replace("D:\\", "");
        //        noDrive = noDrive.Replace("\\", "/");
        //        pathftp = ftpHost + "/" + noDrive;
        //        //For test
        //        if (ModeInt == 1)
        //        { pathftp = ftpHost + "/YingTest/" + noDrive; }

        //        CreateDir(pathftp);

        //        WebRequest ftpRequest = WebRequest.Create(pathftp + "/" + filename);
        //        ftpRequest.Credentials = new NetworkCredential(ftpUser, ftpPassword);
        //        ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
        //        Stream requestStream = ftpRequest.GetRequestStream();
        //        s3file.CopyTo(requestStream);
        //        requestStream.Close();
        //        Writefilelog(string.Format("Backup success detail: Code={0}, Dir={1}", doccode, filepath));
        //        return true;
        //    }
        //    catch (WebException e)
        //    {
        //        FtpWebResponse response3 = (FtpWebResponse)e.Response;
        //        Writefilelog("Backup file to FTP fail: Path=" + filepath + " | Filename:" + filename + " | Error:" + response3.StatusCode);
        //        //Write log db
        //        faillogDB(doccode, filename, "status code: " + response3.StatusCode + " description: " + response3.StatusDescription, ModeStr);
        //        return false;
        //    }
        //}

        private static void CreateDir(string NewDir)
        {
            WebRequest request = WebRequest.Create(NewDir);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = new NetworkCredential(ftpUser, ftpPassword);
            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
            }
        }

        #endregion FTP Zone

        private static void ModeSmileDoc()
        {
            //รอ Database
            Writefilelog("Start process");
            //Get data
            S3UploaderDataContext db = new S3UploaderDataContext(ConnectionStr);
            List<usp_SmileDoclistDelete_SelectResult> itemlst = new List<usp_SmileDoclistDelete_SelectResult>();
            try
            {
                itemlst = db.usp_SmileDoclistDelete_Select().ToList();
            }
            catch (Exception ex)
            {
                Writefilelog("Get list usp_SmileDoclistDelete_Select error detail: " + ex.Message);
                Writefilelog("End process");
                return;
            }

            if (itemlst.Count > 0)
            {
                Writefilelog("Get list total " + string.Format("{0:#,0}", itemlst.Count));

                foreach (var item in itemlst)
                {
                    string DocCode = item.AttachmentID.ToString();
                    //For Test*****************************************************
                    if (ModeInt == 1 || ModeInt == 3)
                    { fortestcopyfile(item.PhysicalPath); }
                    //S3 check
                    s3Bucket = item.S3Bucket;
                    var taskread = ReadObjectDataAsync(DocCode, item.S3Key);
                    taskread.Wait();
                    if (taskread.Result)//Founded the file in s3 storage
                    {
                        //File is exist
                        if (File.Exists(item.PhysicalPath))
                        {
                            //backup
                            bool backupresult = UploadtoFTP(DocCode, item.PhysicalPath, item.FileName);
                            if (backupresult)
                            {
                                //delete a file
                                try
                                {
                                    File.Delete(item.PhysicalPath);
                                    Writefilelog("Delete file success code: " + DocCode + " path: " + item.PhysicalPath);
                                    SmilesuccessUpdateDB(DocCode);//Update db isuploaded
                                    continue;//go to next item
                                }
                                catch (Exception e)
                                {
                                    Writefilelog("Delete file error code: " + DocCode + " path: " + item.PhysicalPath + " message: " + e.Message);
                                    faillogDB(DocCode, item.FileName, "Delete file fail: " + e.Message, ModeStr);
                                    continue;//go to next item
                                }
                            }
                            else { continue;/*go to next item*/}//Update backup fail
                        }//if file is exist
                        else
                        {
                            //Copy file from s3 to FTP by using stream file
                            var taskstream = StreamFileFromS3(item.S3Key, DocCode, item.PhysicalPath);
                            taskstream.Wait();
                            if (taskstream.Result == true)
                            {
                                SmilesuccessUpdateDB(DocCode);//Update db isuploaded
                                continue;/*go to next item*/
                            }
                            else
                            {
                                Writefilelog("File not found on local and stream file from s3 fail code: " + DocCode + " path: " + item.PhysicalPath);
                                faillogDB(DocCode, item.FileName, "File not found on local and stream file from s3 fail", ModeStr);
                                continue;/*go to next item*/
                            }
                        }
                    }
                    else { continue;/*go to next item*/}//not found the file in s3 storage
                }
            }
            else
            {
                Writefilelog("No list now.");
                Writefilelog("End process");
                return;
            }
            Writefilelog("End process");
        }

        private static void ModeSSSDoc()
        {
            Writefilelog("Start process");
            //Get data
            S3UploaderDataContext db = new S3UploaderDataContext(ConnectionStr);
            List<usp_DocumentFileDelete_SelectResult> itemlst = new List<usp_DocumentFileDelete_SelectResult>();
            try
            {
                itemlst = db.usp_DocumentFileDelete_Select().ToList();
            }
            catch (Exception ex)
            {
                Writefilelog("Get list usp_DocumentFileDelete_Select error detail: " + ex.Message);
                Writefilelog("End process");
                return;
            }

            if (itemlst.Count > 0)
            {
                Writefilelog("Get list total " + string.Format("{0:#,0}", itemlst.Count));

                foreach (var item in itemlst)
                {
                    //For Test*****************************************************
                    if (ModeInt == 1)
                    { fortestcopyfile(item.PhysicalPath); }
                    //S3 check
                    s3Bucket = item.S3Bucket;
                    var taskread = ReadObjectDataAsync(item.Code, item.S3Key);
                    taskread.Wait();
                    if (taskread.Result)//Founded the file in s3 storage
                    {
                        //File is exist
                        if (File.Exists(item.PhysicalPath))
                        {
                            //backup
                            bool backupresult = UploadtoFTP(item.Code, item.PhysicalPath, item.FileName);
                            if (backupresult)
                            {
                                //delete a file
                                try
                                {
                                    File.Delete(item.PhysicalPath);
                                    Writefilelog("Delete file success code: " + item.Code + " path: " + item.PhysicalPath);
                                    SSSsuccessUpdateDB(item.Code);//Update db isuploaded
                                    continue;//go to next item
                                }
                                catch (Exception e)
                                {
                                    Writefilelog("Delete file error code: " + item.Code + " path: " + item.PhysicalPath + " message: " + e.Message);
                                    faillogDB(item.Code, item.FileName, "Delete file fail: " + e.Message, ModeStr);
                                    continue;//go to next item
                                }
                            }
                            else { continue;/*go to next item*/}//Update backup fail
                        }//if file is exist
                        else
                        {
                            //Copy file from s3 to FTP by using stream file
                            var taskstream = StreamFileFromS3(item.S3Key, item.Code, item.PhysicalPath);
                            taskstream.Wait();
                            if (taskstream.Result == true)
                            {
                                SSSsuccessUpdateDB(item.Code);//Update db isuploaded
                                continue;/*go to next item*/
                            }
                            else
                            {
                                Writefilelog("File not found on local and stream file from s3 fail code: " + item.Code + " path: " + item.PhysicalPath);
                                faillogDB(item.Code, item.FileName, "File not found on local and stream file from s3 fail", ModeStr);
                                continue;/*go to next item*/
                            }
                        }
                    }
                    else { continue;/*go to next item*/}//not found the file in s3 storage
                }
            }
            else
            {
                Writefilelog("No list now.");
                Writefilelog("End process");
                return;
            }

            Writefilelog("End process");
        }

        private static void Writefilelog(string messstr)
        {
            Console.WriteLine(messstr);
            logging.pathstr = pathstr + @"\log";
            logging.fileName = string.Format(ModeStr + "_Log {0}.log", DateTime.Now.ToString("dd-MM-yyyy"));
            logging.Writelog(messstr);
        }

        private static bool getconfig()
        {
            try
            {
                //Get parameter from database (folloe by mode)
                switch (ModeInt)
                {
                    case 2:
                        ConnectionStr = SmileDocumentBackupAndDelete.Properties.Settings.Default.DBSSSDoc;
                        break;

                    case 4:
                        ConnectionStr = SmileDocumentBackupAndDelete.Properties.Settings.Default.DBSmileDoc;
                        break;

                    default:
                        ConnectionStr = SmileDocumentBackupAndDelete.Properties.Settings.Default.DBTest;
                        break;
                }
                Writefilelog("Connection string =" + ConnectionStr);

                //Get configuration in database
                S3UploaderDataContext DataContext = new S3UploaderDataContext(ConnectionStr);
                List<usp_UploaderConfig_SelectResult> lst = new List<usp_UploaderConfig_SelectResult>(); //Create list result
                lst = DataContext.usp_UploaderConfig_Select(null).ToList();
                foreach (var data in lst)
                {
                    if (data.ParameterName == "S3AccesskeyRead") { s3AccKey_read = data.ValueString; }
                    else if (data.ParameterName == "S3SecretkeyRead") { s3SecKey_read = data.ValueString; }
                    else if (data.ParameterName == "S3BucketName") { s3Bucket = data.ValueString; }
                    else if (data.ParameterName == "FTPPath") { ftpHost = data.ValueString; }
                    else if (data.ParameterName == "FTPUser") { ftpUser = data.ValueString; }
                    else if (data.ParameterName == "FTPPassword") { ftpPassword = data.ValueString; }
                    else if (data.ParameterName == "BackupAndDeleteFrequency") { fretypevalue = data.ValueString; frevalue = (data.ValueNumber).ToString(); }
                    else if (data.ParameterName == "BackupAndDeleteType") { fretype = data.ValueString; }
                    else if (data.ParameterName == "BackupAndDeleteBegin") { fretimebegin = data.ValueString; }
                    else if (data.ParameterName == "BackupAndDeleteEnd") { fretimeend = data.ValueString; }
                }

                lst.Clear();
                DataContext.Connection.Close();
                DataContext = null;
                //Write summary configuration
                //Get config and write log sumary
                Writefilelog(string.Format("Program Version : {0}", Assembly.GetExecutingAssembly().GetName().Version));
                Writefilelog(string.Format("Mode is: {0}", ModeStr));
                Writefilelog(string.Format("Database :" + ConnectionStr));
                Writefilelog(string.Format("sFTP host: {0}, user: {1}, password: {2}", ftpHost, ftpUser, ftpPassword));
                Writefilelog(string.Format("S3 bucket name: {0} acckey:{1} scckey:{2}", s3Bucket, s3AccKey_read, s3SecKey_read));
                Writefilelog(string.Format("Timer begin:{0} end:{1} type:{2} frequency:{3} {4}", fretimebegin, fretimeend, fretype, frevalue, fretypevalue));
                return true;
            }
            catch (Exception ex)
            {   //Set service stop
                Writefilelog("Read configuration error:" + ex.Message);
                return false;
            }
        }

        private static bool faillogDB(string docId, string fileN, string errMes, string docType)
        {
            try
            {
                S3UploaderDataContext db = new S3UploaderDataContext(ConnectionStr);
                var result = db.usp_Uploader_Log_Insert(docId, fileN, errMes, docType).Single();
                if (result.Result != true)
                {
                    Writefilelog(string.Format("Upadate usp_Uploader_Log_Insert fail detail: DocumentID({0}), file name({1}), message({2}), doctype({3})", docId, fileN, errMes, docType));
                    return false;
                }
                Writefilelog("Upadate usp_Uploader_Log_Insert success");
                return true;
            }
            catch (Exception ex)
            {
                Writefilelog(string.Format("Upadate usp_Uploader_Log_Insert error detail: DocumentID({0}), file name({1}), error message({2}), doctype({3})", docId, fileN, ex.Message, docType));
                return false;
            }
        }

        private static bool SSSsuccessUpdateDB(string codestr)
        {
            try
            {
                S3UploaderDataContext db = new S3UploaderDataContext(ConnectionStr);
                var result = db.usp_DocumentFileDelete_Update(codestr).Single();
                if (result.Result == false)
                {
                    Writefilelog(string.Format("Update usp_DocumentFileDelete_Update fail: document code:{0}", codestr));
                    return false;
                }
                Writefilelog(string.Format("Update usp_DocumentFileDelete_Update success: document code:{0}", codestr));
                return true;
            }
            catch (Exception ex)
            {
                Writefilelog(string.Format("Update usp_DocumentFileDelete_Update error:{0} , document code:{1}", ex.Message, codestr));
                return false;
            }
        }

        private static bool SmilesuccessUpdateDB(string codestr)
        {
            try
            {
                S3UploaderDataContext db = new S3UploaderDataContext(ConnectionStr);
                var result = db.usp_SmileDoclistDelete_Update(codestr, true, null, null, null).Single();
                if (result.Result == false)
                {
                    Writefilelog(string.Format("Update usp_SmileDoclistDelete_Update fail: document code:{0}", codestr));
                    return false;
                }
                Writefilelog(string.Format("Update usp_SmileDoclistDelete_Update success: document code:{0}", codestr));
                return true;
            }
            catch (Exception ex)
            {
                Writefilelog(string.Format("Update usp_SmileDoclistDelete_Update error:{0} , document code:{1}", ex.Message, codestr));
                return false;
            }
        }

        public static async Task<bool> ReadObjectDataAsync(string codestr, string keystr)
        {
            //s3Bucket = "p-isc-ss-1-bucketdata";
            S3UploaderDataContext s3 = new S3UploaderDataContext(ConnectionStr);
            GetObjectMetadataRequest request = new GetObjectMetadataRequest
            {
                BucketName = s3Bucket,
                Key = keystr
            };

            try
            {
                IAmazonS3 client = new AmazonS3Client(s3AccKey_read, s3SecKey_read, RegionEndpoint.APSoutheast1);
                GetObjectMetadataResponse response = await client.GetObjectMetadataAsync(request);
                Writefilelog(string.Format("Founded: Code={0} S3Key={1}", codestr, keystr));
                return true;
            }
            catch (AmazonS3Exception e)
            {
                if (e.ErrorCode == "NotFound")
                {
                    Writefilelog(string.Format("Not found file on S3: Code={0} S3Key={1} HTTP status={2}", codestr, keystr, e.ErrorCode));
                    faillogDB(codestr, Path.GetFileName(keystr), "Not found file on S3", ModeStr);
                }
                else
                {
                    Writefilelog(string.Format("Cannot access to S3, please check acckey or scckey or bucket: Code={0} S3Key={1} HTTP status={2}", codestr, keystr, e.ErrorCode));
                    faillogDB(codestr, Path.GetFileName(keystr), "Cannot access to S3, please check acckey or scckey or bucket", ModeStr);
                    return false;
                }

                if (ModeInt == 3)//Smile doc
                {
                    s3.usp_SmileDoclistS3_Update(codestr, false, null, null, null);
                    Writefilelog("Update usp_SmileDoclistS3_Update success!!");
                }
                else
                {//SSSDoc
                    var s3update_re = s3.usp_DocumentFilelistS3_Update(codestr, false, null, null, null).Single();
                    if (s3update_re.Result == true)
                    { Writefilelog("Update usp_DocumentFilelistS3_Update success!!"); }
                    else
                    { Writefilelog("Update usp_DocumentFilelistS3_Update fail~~"); }
                }
                return false;
            }
        }

        private static async Task<bool> StreamFileFromS3(string s3key, string doccode, string filepath)
        {
            string noDrive;
            string filename = Path.GetFileName(filepath);
            string pathftp = "";
            //Prepare parameter
            string dir = Path.GetDirectoryName(filepath);
            noDrive = dir.Replace("D:\\", "");
            noDrive = noDrive.Replace("\\", "/");
            pathftp = ftpHost + "/" + noDrive;
            //For test
            if (ModeInt == 1 || ModeInt == 3)
            { pathftp = ftpHost + "/YingTest/" + noDrive; }

            CreateDir(pathftp);

            WebRequest ftpRequest = WebRequest.Create(pathftp + "/" + filename);
            ftpRequest.Credentials = new NetworkCredential(ftpUser, ftpPassword);
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
            Stream requestStream = ftpRequest.GetRequestStream();

            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = s3Bucket,
                    Key = s3key
                };
                IAmazonS3 client = new AmazonS3Client(s3AccKey_read, s3SecKey_read, RegionEndpoint.APSoutheast1);
                GetObjectResponse response = await client.GetObjectAsync(request);
                MemoryStream memoryStream = new MemoryStream();
                using (Stream responseStream = response.ResponseStream)
                {
                    responseStream.CopyTo(requestStream);
                }
                requestStream.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static int DoConvertModeToInt()
        {
            int return_value = 1; //default is test mode sssdoc
            try
            {
                if (ModeStr.Equals("sssdoc", StringComparison.OrdinalIgnoreCase))
                { return_value = 2; }
                else if (ModeStr.Equals("smiletest", StringComparison.OrdinalIgnoreCase))
                { return_value = 3; }
                else if (ModeStr.Equals("smiledoc", StringComparison.OrdinalIgnoreCase))
                { return_value = 4; }
                ModeInt = return_value;
                return return_value;
            }
            catch { return return_value; }
        }

        private static void StartTimer()
        {
            //prepare parameter
            double fredoubvalue = 20; //defualt
            try { if (frevalue != "") { fredoubvalue = Convert.ToDouble(frevalue); } }
            catch (Exception) { Writefilelog("Convert frequency fail. Frequency default value is 20"); fredoubvalue = 20; }

            _Timer = new ReportTimer();
            _Timer.Elapsed += new ReportEventHandler(_Timer_Elapsed);
            _Timer.Error += new Schedule.ExceptionEventHandler(_Timer_Error);
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
                                   new SimpleInterval(DateTime.Now.AddMinutes(1), Tsp),
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

        private static bool fortestcopyfile(string filepath)
        {
            try
            {
                if (!File.Exists(filepath))
                {
                    if (!Directory.Exists(Path.GetDirectoryName(filepath)))
                    { Directory.CreateDirectory(Path.GetDirectoryName(filepath)); }
                    System.IO.File.Copy("E:\\Test Upload\\610000001.pdf", filepath);
                }
                return true;
            }
            catch (Exception e) { return false; }
        }

        #endregion Method

        #region Event

        private static void _Timer_Error(object sender, ExceptionEventArgs Args)
        {
            Writefilelog("Timer error: " + Args.Error.Message);
            Thread.Sleep(5000);
            Process.GetCurrentProcess().Kill();
        }

        private static void _Timer_Elapsed(object sender, ReportEventArgs e)
        {
            if (isRunning == false)
            {
                isRunning = true;
                DoRunning();
                isRunning = false;
            }
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            aTimer.Enabled = false;
            DoInit();
        }

        public static void DoInit()
        {
            if (ModeStr != "")
            {
                DoConvertModeToInt();
                if (getconfig())//Get configuration
                {
                    bool result = ConnectFTP();
                    if (result)
                    {
                        //Start timer
                        try
                        {
                            StartTimer();//**************************************
                            //DoRunning();//For test
                            Writefilelog("Service is ready");
                        }
                        catch (Exception ex) { Writefilelog("Service not ready, fail to start timer detail: " + ex.Message); }
                    }
                    else
                    {
                        Writefilelog("Service not ready, please check configuration again.");
                        Writefilelog("Service will reconnect again in a minute");
                        if (aTimer != null)
                        { aTimer = null; }
                        aTimer = new System.Timers.Timer(60000);
                        aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                        aTimer.Enabled = true;
                    }
                }
            }
        }

        public static void DoRunning()
        {
            getconfig();//Get configuration
            bool result = ConnectFTP();
            if (result)
            {
                switch (ModeInt)
                {
                    case 2:
                        ModeSSSDoc();
                        break;

                    case 3:
                        ModeSmileDoc();
                        break;

                    case 4:
                        ModeSmileDoc();
                        break;

                    default:
                        ModeSSSDoc();
                        break;
                }
            }
        }

        #endregion Event
    }
}