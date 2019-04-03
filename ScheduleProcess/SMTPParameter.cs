using System;
using System.Data;
using System.Data.SqlClient;

namespace Functions
{
    public class SMTPParameter
    {
        public DataRow getSMTP()
        {
            string _SQL = "SELECT * From SMTPParameter";
            DataTable dt = Common.RunQuery(_SQL);
            if (dt.Rows.Count > 0)
                return dt.Rows[0];
            else return null;
        }
    }
}
