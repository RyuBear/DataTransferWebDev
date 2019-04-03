using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Transfer.Models.Repository
{
    public class tblXMLMappingRepository : GenericRepository<tblXMLMapping>
    {
        public IEnumerable<tblXMLMapping> get(string XMLName)
        {
            IEnumerable<tblXMLMapping> mapping = this.GetSome(x => x.XMLName.Equals(XMLName, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Idx);
            return mapping;
        }

        public bool Delete(string XMLName)
        {
            try
            {
                // 刪除舊有欄位資料
                string _sql = "DELETE FROM tblXMLMapping Where XMLName=@XMLName";
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
        /// 儲存 XML Mapping
        /// </summary>
        /// <returns></returns>
        public string Save(string XMLName, string Creator, List<tblXMLMapping> Mappings)
        {
            try
            {
                if (Delete(XMLName))
                {
                    foreach (tblXMLMapping m in Mappings)
                    {
                        try
                        {
                            m.XMLName = XMLName;
                            m.CreateTime = Now;
                            m.Creator = Creator;
                            this.Create(m);
                        }
                        catch (Exception ex)
                        {
                            Delete(XMLName);
                            return "新增XML設定時發生錯誤! (原因：" + ex.Message + ")";
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
