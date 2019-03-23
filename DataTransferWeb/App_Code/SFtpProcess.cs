using DataTransferWeb.Models;
using Renci.SshNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DataTransferWeb
{
    /// <summary>
    /// SFTP操作類
    /// </summary>
    public class SFtpProcess
    {
        #region 欄位或屬性
        private SftpClient sftp;
        /// <summary>
        /// SFTP連線狀態
        /// </summary>
        public bool Connected { get { return sftp.IsConnected; } }
        #endregion

        #region 構造
        /// <summary>
        /// 構造
        /// </summary>
        /// <param name="ip">IP</param>
        /// <param name="port">埠</param>
        /// <param name="user">使用者名稱</param>
        /// <param name="pwd">密碼</param>
        public SFtpProcess(FTPData data)
        {
            //string ip, string port, string user, string pwd
            sftp = new SftpClient(data.FTPServerIP, data.Port, data.UserName, data.Password);
        }
        #endregion

        #region 連線SFTP
        /// <summary>
        /// 連線SFTP
        /// </summary>
        /// <returns>true成功</returns>
        public bool Connect()
        {
            try
            {
                if (!Connected)
                {
                    sftp.Connect();
                }
                return true;
            }
            catch (Exception ex)
            {
                // TxtLog.WriteTxt(CommonMethod.GetProgramName(), string.Format("連線SFTP失敗，原因：{0}", ex.Message));
                throw new Exception(string.Format("連線SFTP失敗，原因：{0}", ex.Message));
            }
        }
        #endregion

        #region 斷開SFTP
        /// <summary>
        /// 斷開SFTP
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (sftp != null && Connected)
                {
                    sftp.Disconnect();
                }
            }
            catch (Exception ex)
            {
                // TxtLog.WriteTxt(CommonMethod.GetProgramName(), string.Format("斷開SFTP失敗，原因：{0}", ex.Message));
                throw new Exception(string.Format("斷開SFTP失敗，原因：{0}", ex.Message));
            }
        }
        #endregion

        #region SFTP上傳檔案
        /// <summary>
        /// SFTP上傳檔案
        /// </summary>
        /// <param name="localPath">本地路徑</param>
        /// <param name="remotePath">遠端路徑</param>
        public string Put(FileInfo file, string remotePath)
        {
            try
            {
                using (FileStream f = File.OpenRead(file.FullName))
                {
                    Connect();
                    sftp.UploadFile(f, remotePath + "/" + file.Name);
                    Disconnect();
                    return "";
                }
            }
            catch (Exception ex)
            {
                // TxtLog.WriteTxt(CommonMethod.GetProgramName(), string.Format("SFTP檔案上傳失敗，原因：{0}", ex.Message));
                return string.Format("SFTP檔案上傳失敗，原因：{0}", ex.Message);
            }
            finally
            {
                Func.DelAttachment(file);
            }
        }
        #endregion

        #region SFTP獲取檔案
        /// <summary>
        /// SFTP獲取檔案
        /// </summary>
        /// <param name="remotePath">遠端路徑</param>
        /// <param name="localPath">本地路徑</param>
        public void Get(string remotePath, string localPath)
        {
            try
            {
                Connect();
                var byt = sftp.ReadAllBytes(remotePath);
                Disconnect();
                File.WriteAllBytes(localPath, byt);
            }
            catch (Exception ex)
            {
                // TxtLog.WriteTxt(CommonMethod.GetProgramName(), string.Format("SFTP檔案獲取失敗，原因：{0}", ex.Message));
                throw new Exception(string.Format("SFTP檔案獲取失敗，原因：{0}", ex.Message));
            }

        }
        #endregion

        #region 刪除SFTP檔案
        /// <summary>
        /// 刪除SFTP檔案
        /// </summary>
        /// <param name="remoteFile">遠端路徑</param>
        public void Delete(string remoteFile)
        {
            try
            {
                Connect();
                sftp.Delete(remoteFile);
                Disconnect();
            }
            catch (Exception ex)
            {
                // TxtLog.WriteTxt(CommonMethod.GetProgramName(), string.Format("SFTP檔案刪除失敗，原因：{0}", ex.Message));
                throw new Exception(string.Format("SFTP檔案刪除失敗，原因：{0}", ex.Message));
            }
        }
        #endregion
        
        #region 移動SFTP檔案
        /// <summary>
        /// 移動SFTP檔案
        /// </summary>
        /// <param name="oldRemotePath">舊遠端路徑</param>
        /// <param name="newRemotePath">新遠端路徑</param>
        public void Move(string oldRemotePath, string newRemotePath)
        {
            try
            {
                Connect();
                sftp.RenameFile(oldRemotePath, newRemotePath);
                Disconnect();
            }
            catch (Exception ex)
            {
                // TxtLog.WriteTxt(CommonMethod.GetProgramName(), string.Format("SFTP檔案移動失敗，原因：{0}", ex.Message));
                throw new Exception(string.Format("SFTP檔案移動失敗，原因：{0}", ex.Message));
            }
        }
        #endregion

    }
}