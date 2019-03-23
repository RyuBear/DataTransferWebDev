using System;
using System.Collections.Generic;
using System.Linq;

namespace Transfer.Models.Repository
{
    public class bscodeRepository : GenericRepository<bscode>
    {
        /// <summary>
        /// 取得 Sql Type資訊
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public List<bscode> getSqlType()
        {
            IEnumerable<bscode> codes = this.GetSome(x => x.cd_type.Equals("SQL_TYPE"));
            return codes.ToList();
        }

        /// <summary>
        /// 依 Sql Type 取得 EMAIL 主旨
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string getSubject(string cd)
        {
            bscode code = this.Get(x => x.cd_type.Equals("SQL_TYPE") && x.cd.Equals(cd, StringComparison.OrdinalIgnoreCase));
            if (code != null)
                return code.value1;
            else
                return "";
        }
    }
}
