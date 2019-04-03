using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Functions
{
    public static class Common
    {
        #region 屬性
        private static string _connectionString = null;
        private static string ConnectionString
        {
            get
            {
                if (_connectionString == null)
                {
                    ConnectionStringSettings cs = ConfigurationManager.ConnectionStrings["DBConnection"];
                    _connectionString = cs.ConnectionString;
                }
                return _connectionString;
            }
        }
        #endregion

        #region Common Methods
        private static SqlCommand BuildSqlCommand(SqlConnection conn, string sqlText, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(sqlText, conn);
            command.CommandType = CommandType.Text;

            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    command.Parameters.Add(parameter);
                }
            }
            return command;
        }

        public static DataTable RunQuery(string sqlText)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                DataTable ds = new DataTable();
                connection.Open();
                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand command = new SqlCommand(sqlText, connection);
                da.SelectCommand = command;
                da.Fill(ds);
                da.Dispose();
                connection.Close();
                return ds;
            }
        }

        public static DataTable RunQuery(string sqlText, IDataParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                DataTable ds = new DataTable();
                connection.Open();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = BuildSqlCommand(connection, sqlText, parameters);
                da.Fill(ds);
                da.Dispose();
                connection.Close();
                return ds;
            }
        }

        public static bool RunNonQuery(string sqlText, IDataParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                bool ok = false;
                connection.Open();
                SqlCommand command = BuildSqlCommand(connection, sqlText, parameters);
                try
                {
                    command.ExecuteNonQuery();
                    ok = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                command.Cancel();
                connection.Close();
                return ok;
            }
        }

        public static bool RunNonQuery(List<string> sqlTextList, List<IDataParameter[]> parametersList)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                if (sqlTextList.Count != parametersList.Count)
                {
                    return false;
                }
                bool ok = false;
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    for (int i = 0; i < sqlTextList.Count; i++)
                    {
                        SqlCommand command = BuildSqlCommand(connection, sqlTextList[i], parametersList[i]);
                        command.Transaction = trans;
                        command.ExecuteNonQuery();
                    }
                    trans.Commit();
                    ok = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    trans.Rollback();
                }
                connection.Close();
                connection.Dispose();
                return ok;
            }
        }

        public static int GetMaxID(string FieldName, string TableName, string Constraint)
        {
            string strsql = "Select Max(" + FieldName + ") + 1 From " + TableName + Constraint;
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }

        public static object GetSingle(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }
        #endregion

        #region 型態轉換
        public static bool? StringToBoolean(string value)
        {
            try
            {
                return bool.Parse(value);
            }
            catch { return null; }
        }

        public static decimal? StringToDecimal(string value)
        {
            try
            {
                return decimal.Parse(value);
            }
            catch { return null; }
        }

        public static int? StringToInt(string value)
        {
            try
            {
                return int.Parse(value);
            }
            catch { return null; }
        }
        #endregion
    }
}