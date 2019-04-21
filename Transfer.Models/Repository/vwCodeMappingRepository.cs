using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Transfer.Models.Repository
{
    public class vwCodeMappingRepository : GenericRepository<vwCodeMapping>
    {
        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <param name="ModeType"></param>
        /// <param name="Format"></param>
        /// <param name="SettingName"></param>
        /// <param name="FieldName">Tag/Column Name</param>
        /// <returns></returns>
        public List<vwCodeMapping> query(string CustomerName, string ModeType, string Format, string SettingName, string FieldName)
        {
            IQueryable<vwCodeMapping> mappings = this.GetAll();
            if (!string.IsNullOrEmpty(CustomerName)) mappings = this.GetSome(mappings, x => x.CustomerName.Equals(CustomerName, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(ModeType)) mappings = this.GetSome(mappings, x => x.ModeType.Equals(ModeType, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(Format)) mappings = this.GetSome(mappings, x => x.Format.Equals(Format, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(SettingName)) mappings = this.GetSome(mappings, x => x.SettingName.Contains(SettingName));
            if (!string.IsNullOrEmpty(FieldName)) mappings = this.GetSome(mappings, x => x.FieldName.Contains(FieldName));

            return mappings.ToList();
        }

        public vwCodeMapping get(string SettingName, string ModeType, string Format, string FieldName, string BeforeValue)
        {
            vwCodeMapping mapping = this.Get(x => x.SettingName.Equals(SettingName, StringComparison.OrdinalIgnoreCase)
                                                && x.ModeType.Equals(ModeType, StringComparison.OrdinalIgnoreCase)
                                                && x.Format.Equals(Format, StringComparison.OrdinalIgnoreCase)
                                                && x.FieldName.Equals(FieldName, StringComparison.OrdinalIgnoreCase)
                                                && x.BeforeValue.Equals(BeforeValue, StringComparison.OrdinalIgnoreCase));
            return mapping;
        }
    }
}
