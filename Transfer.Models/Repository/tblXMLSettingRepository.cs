using System;
using System.Linq;
using Transfer.Models.ViewModel;
using System.Collections.Generic;
using Transfer.Models.Models;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using LinqKit;

namespace Transfer.Models.Repository
{
    public class tblXMLSettingRepository : GenericRepository<tblXMLSetting>
    {
        /// <summary>
        /// 確認是否已存在
        /// </summary>
        /// <param name="XMLName"></param>
        /// <returns></returns>
        public bool isExist(string XMLName)
        {
            tblXMLSetting setting = this.Get(x => x.XMLName.Equals(XMLName, StringComparison.OrdinalIgnoreCase));
            if (setting == null)
                return false;
            else
                return true;
        }

        public tblXMLSetting get(string XMLName)
        {
            tblXMLSetting setting = this.Get(x => x.XMLName.Equals(XMLName, StringComparison.OrdinalIgnoreCase));
            return setting;
        }

        public IEnumerable<tblXMLSetting> getByCustomer(string UserID, string CustomerName)
        {
            IEnumerable<tblXMLSetting> settings = this.GetSome(x => x.UserId.Contains(UserID) && x.CustomerName.Equals(CustomerName, StringComparison.OrdinalIgnoreCase));
            return settings;
        }

        public List<tblXMLSetting> get(string XMLName, string SQLName, string CustomerName)
        {
            IQueryable<tblXMLSetting> settings = this.GetAll();
            if (!string.IsNullOrEmpty(XMLName)) settings = this.GetSome(settings, x => x.XMLName.Equals(XMLName, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(SQLName)) settings = this.GetSome(settings, x => x.SQLName.Equals(SQLName, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(CustomerName)) settings = this.GetSome(settings, x => x.CustomerName.Equals(CustomerName, StringComparison.OrdinalIgnoreCase));

            return settings.ToList();
        }


        public bool Delete(string XMLName)
        {
            try
            {
                // 刪除舊有欄位資料
                string _sql = @"Delete From tblXMLMapping Where XMLName=@XMLName; 
                                DELETE FROM tblXMLSetting Where XMLName=@XMLName";
                SqlParameter[] ps = new SqlParameter[]
                {
                    new SqlParameter("@XMLName", XMLName.ToSqlType())
                };
                return this.ExecuteNonQuery(_sql, ps);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 儲存 XML Setting
        /// </summary>
        /// <returns></returns>
        public string Save(string XMLName, string CustomerName, string SQLName, string FileName, string DateFormat, string UserID, string Creator, List<tblXMLMapping> Mappings)
        {
            tblXMLSetting setting = this.Get(x => x.XMLName.Equals(XMLName, StringComparison.OrdinalIgnoreCase));
            if (setting == null)
            {
                setting = new tblXMLSetting()
                {
                    XMLName = XMLName,
                    CustomerName = CustomerName,
                    SQLName = SQLName,
                    FileName = FileName,
                    FileNameDateFormat = DateFormat,
                    UserId = "," + UserID + ",",
                    CreateTime = Now,
                    Creator = Creator
                };
                try
                {
                    this.Create(setting);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            else
            {
                setting.SQLName = SQLName;
                setting.CustomerName = CustomerName;
                setting.FileName = FileName;
                setting.FileNameDateFormat = DateFormat;
                setting.UserId = "," + UserID + ",";
                setting.UpdateTime = Now;
                setting.Updator = Creator;

                try
                {
                    this.Update(setting);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            // Save Mapping
            using (tblXMLMappingRepository map = new tblXMLMappingRepository())
            {
                return map.Save(XMLName, Creator, Mappings);
            }
        }
    }
}
