using System.IO;

namespace ScheduleProcess.Models
{
    public class FTPData
    {
        public string FTPServerIP { get; set; }

        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string DirName { get; set; }        // FTP 檔案路徑

        public bool SSL { get; set; }

        public FileInfo file { get; set; }

        public FTPData()
        {
            DirName = @"/";
            Port = 21;
        }
    }
}
