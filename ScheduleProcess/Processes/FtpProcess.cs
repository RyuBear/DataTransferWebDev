using Extensions;
using System;
using ScheduleProcess.Models;

namespace ScheduleProcess.Processes
{
    public class FtpProcess
    {
        public string Upload(FTPData data)
        {
            /* 獲得日期 */

            try
            {
                FTPExtensions.sFTPServerIP = data.FTPServerIP;
                FTPExtensions.sPort = data.Port;
                FTPExtensions.sUserName = data.UserName;
                FTPExtensions.sPassWord = data.Password;
                FTPExtensions.sDirName = data.DirName; /* FTP路徑 */
                FTPExtensions.sFromFileName = data.file.FullName;
                FTPExtensions.sToFileName = data.file.Name;
                return FTPExtensions.Upload();
            }
            catch (Exception ex)
            {
                return ex.Message.Replace("\r\n", "");
            }
        }
    }
}