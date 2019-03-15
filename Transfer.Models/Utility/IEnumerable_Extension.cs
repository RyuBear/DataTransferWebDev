using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Data;

namespace Transfer.Models {
    public static class IEnumerable_Extension {
        /// <summary>
        /// 將 IEnumerable 轉為 DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> collection)
        {
            var dtReturn = new DataTable();

            // column names
            var oProps = typeof(T).GetProperties();
            foreach (var pi in oProps)
            {
                var colType = pi.PropertyType;
                if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    colType = colType.GetGenericArguments()[0];
                }
                dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
            }

            // Could add a check to verify that there is an element 0
            foreach (var rec in collection)
            {
                var dr = dtReturn.NewRow();
                foreach (var pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) ?? DBNull.Value;
                }
                dtReturn.Rows.Add(dr);
            }

            return (dtReturn);
        }
    
    }
}