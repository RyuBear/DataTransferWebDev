using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleProcess.Models
{
    public class EmailData
    {
        public string Email { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string SMTP { get; set; }
        public int Port { get; set; }
        public bool SSL { get; set; }
        public string DisplayName { get; set; }
        public string To { get; set; }
        public string BCC { get; set; }
        public string CC { get; set; }

        public string Subject { get; set; }
        public string Context { get; set; }
        public FileInfo Attachment { get; set; }
    }
}
