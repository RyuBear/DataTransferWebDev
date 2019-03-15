using System;
using System.Linq;
using Transfer.Models.ViewModel;
using System.Collections.Generic;
using Transfer.Models.Models;
using System.Data;
using System.Data.SqlClient;

namespace Transfer.Models.Repository
{
    public class tblSQLColumnsRepository : GenericRepository<tblSQLColumns>
    {
        public IEnumerable<tblSQLColumns> getAllColumns(string SQLName)
        {
            IEnumerable<tblSQLColumns> columns = this.GetSome(x => x.SQLName.Equals(SQLName, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Idx);
            return columns;
        }

        public bool Delete(string SQLName)
        {
            try
            {
                // 刪除舊有欄位資料
                string _sql = "DELETE FROM tblSQLColumns Where SQLName=@SQLName";
                SqlParameter[] ps = new SqlParameter[]
                {
                    new SqlParameter("@SQLName", SQLName.ToSqlType())
                };
                return this.ExecuteNonQuery(_sql, ps);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 儲存 SQL Column
        /// </summary>
        /// <returns></returns>
        public string Save(string SQLName, List<ColumnData> Columns)
        {
            try
            {
                if (Delete(SQLName))
                {
                    foreach (ColumnData c in Columns)
                    {
                        tblSQLColumns col = new tblSQLColumns()
                        {
                            SQLName = SQLName,
                            ColumnName = c.ColumnName,
                            Idx = c.Idx,
                        };
                        try
                        {
                            this.Create(col);
                        }
                        catch (Exception ex)
                        {
                            Delete(SQLName);
                            return "新增欄位時發生錯誤! (原因：" + ex.Message + ")";
                        }
                    }
                    return "ok";
                }
                else
                    return "清除舊有欄位時發生錯誤!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
