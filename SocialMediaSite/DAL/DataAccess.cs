using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Types;

namespace DAL
{
    public class DataAccess
    {
        public DataTable Execute(string cmdText, List<ParmStruct> parms = null, CommandType cmdType = CommandType.StoredProcedure)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = CreateCommand(SQLCleaner(cmdText), cmdType, parms);

            using (cmd.Connection)
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public int ExecuteNonQuery(string cmdText, List<ParmStruct> parms = null, CommandType cmdType = CommandType.StoredProcedure)
        {
            int rowsAffected;
            SqlCommand cmd = CreateCommand(SQLCleaner(cmdText), cmdType, parms);

            using (cmd.Connection)
            {
                cmd.Connection.Open();
                rowsAffected = cmd.ExecuteNonQuery();

                UnloadParms(parms, cmd);
            }

            return rowsAffected;
        }

        public object ExecuteScaler(string cmdText, List<ParmStruct> parms = null, CommandType cmdType = CommandType.StoredProcedure)
        {
            object retVal;
            SqlCommand cmd = CreateCommand(cmdText, cmdType, parms);

            using (cmd.Connection)
            {
                cmd.Connection.Open();
                retVal = cmd.ExecuteScalar();
            }

            return retVal;
        }

        private SqlCommand CreateCommand(string cmdText, CommandType cmdType, List<ParmStruct> parms)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SocialMediaSite2022ConnectionString"].ConnectionString);

            SqlCommand cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = cmdType;

            if (parms != null)
            {
                foreach (ParmStruct p in parms)
                {
                    cmd.Parameters.Add(p.Name, p.DataType, p.Size).Value = p.Value;
                    cmd.Parameters[p.Name].Direction = p.Direction;
                }
            }

            return cmd;
        }

        private static void UnloadParms(List<ParmStruct> parms, SqlCommand cmd)
        {
            if (parms != null)
            {
                for (int i = 0; i <= parms.Count - 1; i++)
                {
                    ParmStruct p = parms[i];
                    p.Value = cmd.Parameters[i].Value;
                    parms[i] = p;
                }
            }
        }

        private string SQLCleaner(string sql)
        {
            while (sql.Contains("  "))
                sql = sql.Replace("  ", " ");

            return sql.Replace(Environment.NewLine, "");
        }
    }
}
