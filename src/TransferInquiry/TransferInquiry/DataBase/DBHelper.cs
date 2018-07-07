using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferInquiry.DataBase
{
    public class DBHelper
    {
        private static string connStr = "Server=.;Database=TransferInquiry;User Id=sa;Password=123456;";

        protected static int ExecuteNonQuery(string sql, params SqlParameter[] parames)
        {
            using (var con = new SqlConnection(connStr))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.AddRange(parames);

                    var n = cmd.ExecuteNonQuery();
                    
                    return n;
                }
            }
        }

        protected static DataTable GetDataTable(string sql, params SqlParameter[] parames)
        {
            using (var con = new SqlConnection(connStr))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.AddRange(parames);

                    var ad = new SqlDataAdapter(cmd);
                    var set = new DataSet();
                    ad.Fill(set);

                    return set.Tables[0];
                }
            }
        }

        protected static void BulkToDB(DataTable dt, string tableName)
        {
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (var bulk = new SqlBulkCopy(conn))
                {
                    bulk.DestinationTableName = tableName;
                    bulk.BatchSize = dt.Rows.Count;

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        bulk.WriteToServer(dt);
                    }
                }
            }
        }

        protected static void TruncateTable(string tableName)
        {
            using (var con = new SqlConnection(connStr))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    var sql = "TRUNCATE TABLE " + tableName;
                    cmd.CommandText = sql;

                    var n = cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
