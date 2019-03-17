using System;
using System.Linq;
using Transfer.Models.ViewModel;
using System.Collections.Generic;
using Transfer.Models.Models;
using System.Data;
using System.Data.SqlClient;

namespace Transfer.Models.Repository
{
    public class tblSQLSettingRepository : GenericRepository<tblSQLSetting>
    {
        /// <summary>
        /// 取得SQL Setting
        /// </summary>
        /// <param name="SQLName"></param>
        /// <param name="SQLType"></param>
        /// <returns></returns>
        public List<tblSQLSetting> get(string SQLName, string SQLType)
        {
            IQueryable<tblSQLSetting> settings = this.GetAll();
            if (!string.IsNullOrEmpty(SQLName)) settings = this.GetSome(settings, x => x.SQLName.Equals(SQLName, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(SQLType)) settings = this.GetSome(settings, x => x.SQLType.Equals(SQLType, StringComparison.OrdinalIgnoreCase));

            return settings.ToList();
        }

        /// <summary>
        /// 確認是否已存在
        /// </summary>
        /// <param name="SQLName"></param>
        /// <returns></returns>
        public bool isExist(string SQLName)
        {
            tblSQLSetting setting = this.Get(x => x.SQLName.Equals(SQLName, StringComparison.OrdinalIgnoreCase));
            if (setting == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 取得SQL Setting
        /// </summary>
        /// <param name="SQLName"></param>
        /// <returns></returns>
        public tblSQLSetting select(string SQLName)
        {
            tblSQLSetting setting = this.Get(x => x.SQLName.Equals(SQLName, StringComparison.OrdinalIgnoreCase));
            return setting;
        }

        /// <summary>
        /// 儲存 SQL Setting
        /// </summary>
        /// <returns></returns>
        public string Save(string SQLName, string SQLStatement, int DataRow, string SQLType, List<ColumnData> Columns, string Creator)
        {
            tblSQLSetting setting = this.Get(x => x.SQLName.Equals(SQLName, StringComparison.OrdinalIgnoreCase));
            if (setting == null)
            {
                setting = new tblSQLSetting()
                {
                    SQLName = SQLName,
                    SQLStatement = SQLStatement,
                    DataRow = DataRow,
                    SQLType = SQLType,
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
                setting.SQLStatement = SQLStatement;
                setting.DataRow = DataRow;
                setting.SQLType = SQLType;
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

            // Save Column
            using (tblSQLColumnsRepository col = new tblSQLColumnsRepository())
            {
                return col.Save(SQLName, Columns);
            }
        }

        public string Delete(string SQLName)
        {
            try
            {
                // 刪除舊有欄位資料
                string _sql = @"DELETE From tblSQLColumns Where SQLName=@SQLName; 
                                DELETE FROM tblSQLSetting Where SQLName=@SQLName";
                SqlParameter[] ps = new SqlParameter[]
                {
                    new SqlParameter("@SQLName", SQLName.ToSqlType())
                };
                if (this.ExecuteNonQuery(_sql, ps))
                    return "ok";
                else
                    return "刪除失敗(已有 XML/Excel設定指定該SQL";
            }
            catch (Exception ex)
            {
                return ex.Message.Replace("\r\n", "");
            }
        }
    }
}
