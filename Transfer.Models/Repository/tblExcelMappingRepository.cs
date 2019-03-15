using System;
using System.Linq;
using Transfer.Models.ViewModel;
using System.Collections.Generic;
using Transfer.Models.Models;
using System.Data;
using System.Data.SqlClient;

namespace Transfer.Models.Repository
{
    public class tblExcelMappingRepository : GenericRepository<tblExcelMapping>
    {
        public IEnumerable<tblExcelMapping> get(string ExcelName)
        {
            IEnumerable<tblExcelMapping> mapping = this.GetSome(x => x.ExcelName.Equals(ExcelName, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.X);
            return mapping;
        }

        public bool Delete(string ExcelName)
        {
            try
            {
                // 刪除舊有欄位資料
                string _sql = "DELETE FROM tblExcelMapping Where ExcelName=@ExcelName";
                SqlParameter[] ps = new SqlParameter[]
                {
                    new SqlParameter("@ExcelName", ExcelName.ToSqlType())
                };
                return this.ExecuteNonQuery(_sql, ps);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 儲存 Excel Mapping
        /// </summary>
        /// <returns></returns>
        public string Save(string ExcelName, string Creator, List<tblExcelMapping> Mappings)
        {
            try
            {
                if (Delete(ExcelName))
                {
                    foreach (tblExcelMapping m in Mappings)
                    {
                        try
                        {
                            m.ExcelName = ExcelName;
                            this.Create(m);
                        }
                        catch (Exception ex)
                        {
                            Delete(ExcelName);
                            return "新增Excel設定時發生錯誤! (原因：" + ex.Message + ")";
                        }
                    }
                    return "ok";
                }
                else
                    return "清除既有設定時發生錯誤!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
