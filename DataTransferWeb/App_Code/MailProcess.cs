using System.Net.Mail;
using System.Text;
using DataTransferWeb.Models;
using Transfer.Models.Repository;
using Transfer.Models;
using System.Configuration;

namespace DataTransferWeb
{
    public class MailProcess
    {
        /// <summary>
        /// 寄發郵件
        /// </summary>
        /// <param name="To">收件人</param>
        /// <param name="CC">副本</param>
        /// <param name="BCC">密件副本</param>
        /// <param name="subject">主旨</param>
        /// <param name="context">內容</param>
        /// <param name="toManager">是否以密件副本寄給管理者</param>
        public string SendEmail(EmailData data)
        {
            using (tblSMTPRepository rep = new tblSMTPRepository())
            {
                tblSMTP smtp = rep.get(ConfigurationManager.AppSettings["Mail"].ToString());                
                data.Email = smtp.Email;
                data.Account = smtp.Account;
                data.DisplayName = smtp.DisplayName;
                data.SMTP = smtp.SMTP;
                data.Password = smtp.Password;
                data.Port = smtp.Port;
                data.SSL = smtp.SSL;
            }
            MailMessage msg = new MailMessage();
            if (data.To.Length > 0)
                msg.To.Add(data.To);//收件者，以逗號分隔不同收件者
            if (!string.IsNullOrEmpty(data.CC) && data.CC.Length > 0)
                msg.CC.Add(data.CC);//副本
            if (!string.IsNullOrEmpty(data.BCC) && data.BCC.Length > 0)
                msg.Bcc.Add(data.BCC);//密件副本

            //3個參數分別是發件人地址（可以隨便寫），發件人姓名，編碼
            msg.From = new MailAddress(data.Email, data.DisplayName, System.Text.Encoding.UTF8);

            msg.Subject = data.Subject;//郵件標題 
            msg.SubjectEncoding = System.Text.Encoding.UTF8;//郵件標題編碼 
            msg.Body = data.Context;//郵件內容 
            msg.BodyEncoding = System.Text.Encoding.UTF8;//郵件內容編碼 
            msg.IsBodyHtml = false;//是否是HTML郵件 
            msg.Priority = MailPriority.Normal;//郵件優先級 

            #region 設定附件檔案(Attachment)
            string myMailEncoding = "utf-8";
            System.Net.Mail.Attachment attachment1 = new System.Net.Mail.Attachment(data.Attachment.FullName);
            attachment1.Name = System.IO.Path.GetFileName(data.Attachment.FullName);
            attachment1.NameEncoding = Encoding.GetEncoding(myMailEncoding);
            attachment1.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;

            //// 設定該附件為一個內嵌附件(Inline Attachment)
            //attachment1.ContentDisposition.Inline = true;           
            //attachment1.ContentDisposition.DispositionType = System.Net.Mime.DispositionTypeNames.Inline;

            msg.Attachments.Add(attachment1);
            #endregion

            try
            {
                //建立 SmtpClient 物件 並設定 Gmail的smtp主機及Port 
                SmtpClient MySmtp = new SmtpClient(data.SMTP, data.Port);

                //設定你的帳號密碼
                MySmtp.Credentials = new System.Net.NetworkCredential(data.Account, data.Password);

                //Gmial 的 smtp 使用 SSL
                MySmtp.EnableSsl = data.SSL;

                //發送Email
                MySmtp.Send(msg);

                return "";
            }
            catch (System.Net.Mail.SmtpException ex)
            {               
                return ex.Message.Replace("\r\n", "");
            }
            finally
            {
                msg.Dispose();
                Func.DelAttachment(data.Attachment);
            }
        }
    }
}