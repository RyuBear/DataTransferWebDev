using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Net;
using Transfer.Models.Repository;
using System.Web;
using FluentFTP;
using System.Security.Authentication;

namespace Extensions
{
    class FTPExtensions
    {
        tblLogRepository rep = new tblLogRepository();

        private static string _sFTPServerIP, _sUserName, _sPassWord, _sDirName, _sFileOrDir, _sFromFileName, _sToFileName, _sFileName;
        private static int _sPort, _iFTPReTry;

        public static string sFTPServerIP { get { return _sFTPServerIP; } set { _sFTPServerIP = value; } }
        public static int sPort { get { return _sPort; } set { _sPort = value; } }
        public static string sUserName { get { return _sUserName; } set { _sUserName = value; } }
        public static string sPassWord { get { return _sPassWord; } set { _sPassWord = value; } }
        public static string sDirName { get { return _sDirName; } set { _sDirName = value; } }
        public static string sFileOrDir { get { return _sFileOrDir; } set { _sFileOrDir = value; } }
        public static string sFromFileName { get { return _sFromFileName; } set { _sFromFileName = value; } }
        public static string sToFileName { get { return _sToFileName; } set { _sToFileName = value; } }
        public static string sFileName { get { return _sFileName; } set { _sFileName = value; } }
        public static int iFTPReTry { get { return _iFTPReTry; } set { _iFTPReTry = value; } }

        /// ftp上傳
        public static string Upload()
        {
            try
            {
                // create an FTP client
                FtpClient client = new FtpClient();
                client.Host = sFTPServerIP;
                // if you don't specify login credentials, we use the "anonymous" user account
                client.Credentials = new NetworkCredential(sUserName, sPassWord);
                client.Port = sPort;
                //client.SslProtocols = SslProtocols.Tls;

                // begin connecting to the server
                client.Connect();

                // upload a file
                client.UploadFile(sFromFileName, "/" + sDirName + sToFileName);

                client.Disconnect();
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}