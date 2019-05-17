using Functions;
using NPOI.HSSF.UserModel;
using ScheduleProcess.Models;
using ScheduleProcess.Processes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Transfer.Models;
using Transfer.Models.Repository;

namespace ScheduleProcess
{
    class Program
    {
        public static tblLogRepository log = new tblLogRepository();

        static void Main(string[] args)
        {
            // 取得執行時間
            DateTime execTime = DateTime.Now;

            using (tblScheduleRepository rep = new tblScheduleRepository())
            {
                // 取得 DAY Schedule 清單
                List<tblSchedule> dayList = rep.GetDaySchedule(execTime);
                foreach (tblSchedule s in dayList)
                {
                    Generate(s);
                }

                // 取得 DATE Schedule 清單
                List<tblSchedule> dateList = rep.GetDateSchedule(execTime);
                foreach (tblSchedule s in dateList)
                {
                    Generate(s);
                }
            }
        }

        static void Generate(tblSchedule s)
        {
            string exeResult = string.Empty;
            string FileName = string.Empty;

            try
            {
                string SQLName = string.Empty;
                string[] Destinations = s.Destination.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                #region XML
                if (s.Format.Equals("XML"))
                {
                    using (tblXMLSettingRepository rep = new tblXMLSettingRepository())
                    using (tblXMLMappingRepository map = new tblXMLMappingRepository())
                    {
                        tblXMLSetting setting = rep.get(s.SettingName);
                        if (setting != null)
                        {
                            SQLName = setting.SQLName;
                            FileName = setting.FileName + ((string.IsNullOrEmpty(setting.FileNameDateFormat)) ? ".xml" : DateTime.Now.ToString(setting.FileNameDateFormat.Replace(",", "")) + ".xml");
                        }
                        List<tblXMLMapping> mapping = map.get(s.SettingName).ToList();
                        using (tblSQLSettingRepository set = new tblSQLSettingRepository())
                        {
                            tblSQLSetting sqlSetting = set.select(SQLName);
                            using (DataAccess da = new DataAccess())
                            {
                                Tuple<bool, DataTable, string> result = da.TryExecuteDataTable(sqlSetting.SQLStatement.Replace("\r\n", " "), null, null);
                                if (!result.Item1)
                                {
                                    log.Save("轉出", "Schedule", s.CustomerName, s.Format, "", s.Email, s.FTPServer, FileName, "失敗", result.Item3);
                                }

                                XmlDocument xmlDoc = XmlProcess.GenerateXML(result.Item2, mapping);
                                // Path
                                if (Destinations.Contains("1"))
                                {
                                    try
                                    {
                                        xmlDoc.Save(s.Path + "/" + FileName);
                                    }
                                    catch (Exception ex)
                                    {
                                        exeResult = ex.Message.Replace("\r\n", "");
                                    }
                                    if (!string.IsNullOrEmpty(exeResult))
                                        log.Save("轉出", "Schedule", s.CustomerName, s.Format, "PATH", s.Email, s.FTPServer, FileName, "失敗", exeResult);
                                    else
                                        log.Save("轉出", "Schedule", s.CustomerName, s.Format, "PATH", s.Email, s.FTPServer, FileName, "成功", "");
                                }

                                if (Destinations.Contains("2") || Destinations.Contains("3"))
                                {
                                    xmlDoc.Save(System.AppDomain.CurrentDomain.BaseDirectory + "/Files/" + FileName);
                                }

                                // Email
                                if (Destinations.Contains("2"))
                                {
                                    string subject = string.Empty;
                                    using (bscodeRepository bscode = new bscodeRepository())
                                    {
                                        subject = bscode.getSubject(sqlSetting.SQLType);
                                    }
                                    MailProcess sender = new MailProcess();
                                    EmailData mailData = new EmailData()
                                    {
                                        To = s.Email,
                                        Subject = subject,
                                        Attachment = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + "/Files/" + FileName)
                                    };
                                    exeResult = sender.SendEmail(mailData);
                                    if (!string.IsNullOrEmpty(exeResult))
                                        log.Save("轉出", "Schedule", s.CustomerName, s.Format, "EMail", s.Email, s.FTPServer, FileName, "失敗", exeResult);
                                    else
                                        log.Save("轉出", "Schedule", s.CustomerName, s.Format, "EMail", s.Email, s.FTPServer, FileName, "成功", "");
                                }
                                // FTP
                                if (Destinations.Contains("3"))
                                {
                                    FTPData ftpData = new FTPData()
                                    {
                                        FTPServerIP = s.FTPServer,
                                        Port = 21,
                                        UserName = s.FTPAccount,
                                        Password = s.FTPPassword,
                                        file = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + "/Files/" + FileName)
                                    };
                                    FtpProcess uploader = new FtpProcess();
                                    exeResult = uploader.Upload(ftpData);
                                    if (!string.IsNullOrEmpty(exeResult))
                                        log.Save("轉出", "Schedule", s.CustomerName, s.Format, "FTP", s.Email, s.FTPServer, FileName, "失敗", exeResult);
                                    else
                                        log.Save("轉出", "Schedule", s.CustomerName, s.Format, "FTP", s.Email, s.FTPServer, FileName, "成功", "");
                                }
                            }
                        }
                    }
                }
                #endregion
                #region EXCEL
                else if (s.Format.Equals("EXCEL"))
                {
                    using (tblExcelSettingRepository rep = new tblExcelSettingRepository())
                    using (tblExcelMappingRepository map = new tblExcelMappingRepository())
                    {
                        tblExcelSetting setting = rep.get(s.SettingName);
                        if (setting != null)
                        {
                            SQLName = setting.SQLName;
                            FileName = setting.FileName + ((string.IsNullOrEmpty(setting.FileNameDateFormat)) ? ".xls" : DateTime.Now.ToString(setting.FileNameDateFormat.Replace(",", "")) + ".xls");
                        }

                        List<tblExcelMapping> mapping = map.get(s.SettingName).ToList();
                        using (tblSQLSettingRepository set = new tblSQLSettingRepository())
                        {
                            tblSQLSetting sqlSetting = set.select(SQLName);
                            using (DataAccess da = new DataAccess())
                            {
                                Tuple<bool, DataTable, string> result = da.TryExecuteDataTable(sqlSetting.SQLStatement.Replace("\r\n", " "), null, null);
                                if (!result.Item1)
                                {
                                    log.Save("轉出", "Schedule", s.CustomerName, s.Format, "", s.Email, s.FTPServer, FileName, "失敗", result.Item3);
                                }

                                HSSFWorkbook book = ExcelProcess.GenerateExcel(result.Item2, mapping);

                                // Path
                                if (Destinations.Contains("1"))
                                {
                                    FileStream file = new FileStream(s.Path + "\\" + FileName, FileMode.Create);//產生檔案
                                    book.Write(file);
                                    file.Close();
                                    if (!string.IsNullOrEmpty(exeResult))
                                        log.Save("轉出", "Schedule", s.CustomerName, s.Format, "PATH", s.Email, s.FTPServer, FileName, "失敗", exeResult);
                                    else
                                        log.Save("轉出", "Schedule", s.CustomerName, s.Format, "PATH", s.Email, s.FTPServer, FileName, "成功", "");
                                }
                                if (Destinations.Contains("2") || Destinations.Contains("3"))
                                {
                                    FileStream file = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory + "/Files/" + FileName, FileMode.Create);//產生檔案
                                    book.Write(file);
                                    file.Close();
                                }
                                // EMail
                                if (Destinations.Contains("2"))
                                {
                                    string subject = string.Empty;
                                    using (bscodeRepository bscode = new bscodeRepository())
                                    {
                                        subject = bscode.getSubject(sqlSetting.SQLType);
                                    }
                                    MailProcess sender = new MailProcess();
                                    EmailData mailData = new EmailData()
                                    {
                                        To = s.Email,
                                        Subject = subject,
                                        Attachment = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + "/Files/" + FileName)
                                    };
                                    exeResult = sender.SendEmail(mailData);
                                    if (!string.IsNullOrEmpty(exeResult))
                                        log.Save("轉出", "Schedule", s.CustomerName, s.Format, "EMail", s.Email, s.FTPServer, FileName, "失敗", exeResult);
                                    else
                                        log.Save("轉出", "Schedule", s.CustomerName, s.Format, "EMail", s.Email, s.FTPServer, FileName, "成功", "");
                                }
                                // FTP
                                if (Destinations.Contains("3"))
                                {
                                    FTPData ftpData = new FTPData()
                                    {
                                        FTPServerIP = s.FTPServer,
                                        Port = 21,
                                        UserName = s.FTPAccount,
                                        Password = s.FTPPassword,
                                        file = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + "/Files/" + FileName)
                                    };
                                    FtpProcess uploader = new FtpProcess();
                                    exeResult = uploader.Upload(ftpData);
                                    if (!string.IsNullOrEmpty(exeResult))
                                        log.Save("轉出", "Schedule", s.CustomerName, s.Format, "FTP", s.Email, s.FTPServer, FileName, "失敗", exeResult);
                                    else
                                        log.Save("轉出", "Schedule", s.CustomerName, s.Format, "FTP", s.Email, s.FTPServer, FileName, "成功", "");
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                exeResult = ex.Message.Replace("\r\n", "");
                log.Save("轉出", "Schedule", s.CustomerName, s.Format, "", s.Email, s.FTPServer, FileName, "失敗", exeResult);
            }
            finally
            {
                FileInfo file = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + "/Files/" + FileName);
                Func.DelAttachment(file);
            }
        }
    }
}
