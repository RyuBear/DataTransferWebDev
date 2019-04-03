using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace ScheduleProcess
{
    public static class Func
    {
        public static void DelAttachment(FileInfo file)
        {
            try
            {
                if (File.Exists(file.FullName))
                {
                    file.Delete();
                }
            }
            catch { };
        }
    }
}