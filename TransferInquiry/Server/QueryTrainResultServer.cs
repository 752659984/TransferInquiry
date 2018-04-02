using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferInquiry.DataBase;
using TransferInquiry.Entitys;

namespace TransferInquiry.Server
{
    public class QueryTrainResultServer : DBHelper
    {
        public static int InsertQueryTrainResult(QueryTrainResult entity)
        {
            var sql = "INSERT INTO QueryTrainResult(TrainNo,TrainNumber,StartCode,EndCode,QueryStartCode,QueryEndCode,GoTime,ComeTime,TimeSpan) ";
            sql += "VALUES(@TrainNo,@TrainNumber,@StartCode,@EndCode,@QueryStartCode,@QueryEndCode,@GoTime,@ComeTime,@TimeSpan)";

            var ps = new SqlParameter[]
            {
                new SqlParameter("@TrainNo", ((object)entity.TrainNo) ?? DBNull.Value),
                new SqlParameter("@TrainNumber", ((object)entity.TrainNumber) ?? DBNull.Value),
                new SqlParameter("@StartCode", ((object)entity.StartCode) ?? DBNull.Value),
                new SqlParameter("@EndCode", ((object)entity.EndCode) ?? DBNull.Value),
                new SqlParameter("@QueryStartCode", ((object)entity.QueryStartCode) ?? DBNull.Value),
                new SqlParameter("@QueryEndCode", ((object)entity.QueryEndCode) ?? DBNull.Value),
                new SqlParameter("@GoTime", ((object)entity.GoTime) ?? DBNull.Value),
                new SqlParameter("@ComeTime", ((object)entity.ComeTime) ?? DBNull.Value),
                new SqlParameter("@TimeSpan", ((object)entity.TimeSpan) ?? DBNull.Value)
            };

            return ExecuteNonQuery(sql, ps);
        }

        public static bool ExistsQueryTrainResult(string trainNumber, string queryStart, string queryEnd)
        {
            var sql = "SELECT 1 FROM QueryTrainResult WHERE [TrainNumber]=@TrainNumber AND [QueryStartCode]=@QueryStartCode AND [QueryEndCode]=@QueryEndCode";
            var ps = new SqlParameter[]
            {
                new SqlParameter("@TrainNumber", trainNumber),
                new SqlParameter("@QueryStartCode", queryStart),
                new SqlParameter("@QueryEndCode", queryEnd)
            };

            return GetDataTable(sql, ps).Rows.Count > 0;
        }


        public static DataTable StartsWithNameByStart(string stationName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT q.* FROM QueryTrainResult q ");
            sb.Append("INNER JOIN Station s1 ON q.QueryStartCode=s1.SC ");
            sb.Append(string.Format("WHERE s1.FN LIKE '{0}%' ", stationName));

            return GetDataTable(sb.ToString());
        }

        public static DataTable StartsWithNameByEnd(string stationName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT q.* FROM QueryTrainResult q ");
            sb.Append("INNER JOIN Station s1 ON q.QueryEndCode=s1.SC ");
            sb.Append(string.Format("WHERE s1.FN LIKE '{0}%' ", stationName));

            return GetDataTable(sb.ToString());
        }

        public static DataTable StartsWithName(string startStationName, string endStationName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT q.* FROM QueryTrainResult q ");
            sb.Append("INNER JOIN Station s1 ON q.QueryStartCode=s1.SC ");
            sb.Append("INNER JOIN Station s2 ON q.QueryEndCode=s2.SC ");
            sb.Append(string.Format("WHERE s1.FN LIKE '{0}%' AND s2.FN LIKE '{1}%' ", startStationName, endStationName));

            return GetDataTable(sb.ToString());
        }

        public static DataTable StartsWithCode(string startStationCode, string endStationCode)
        {
            var sql = "SELECT * FROM QueryTrainResult WHERE [QueryStartCode]=@QueryStartCode AND [QueryEndCode]=@QueryEndCode";
            var ps = new SqlParameter[]
            {
                new SqlParameter("@QueryStartCode", startStationCode),
                new SqlParameter("@QueryEndCode", endStationCode)
            };

            return GetDataTable(sql, ps);
        }
    }
}
