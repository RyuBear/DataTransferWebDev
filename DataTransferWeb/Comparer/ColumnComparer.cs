using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Transfer.Models;

namespace DataTransferWeb.Comparer
{
    public class ColumnComparer : IEqualityComparer<tblSQLColumns>
    {
        public bool Equals(tblSQLColumns x, tblSQLColumns y)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the Employ' properties are equal.
            return x.SQLName == y.SQLName && x.ColumnName == y.ColumnName;
        }

        public int GetHashCode(tblSQLColumns e)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(e, null)) return 0;

            //Get hash code for the EmpNO field.
            int hashSQLName = e.SQLName == null ? 0 : e.SQLName.GetHashCode();

            //Get hash code for the EmpName field.
            int hashColumnName = e.ColumnName == null ? 0 : e.ColumnName.GetHashCode();

            //Calculate the hash code for the Employ.
            return hashSQLName ^ hashColumnName;
        }
    }
}