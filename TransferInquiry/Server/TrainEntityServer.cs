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
    public class TrainEntityServer: DBHelper
    {
        public static int InsertTrain(TrainEntity entity)
        {
            var sql = "INSERT INTO Train([TrainNumber],[StartStation],[EndStation],[GoTime],[ComeTime],[TimeSpan]) ";
            sql += "VALUES(@TrainNumber,@StartStation,@EndStation,@GoTime,@ComeTime,@TimeSpan)";

            var ps = new SqlParameter[]
            {
                new SqlParameter("@TrainNumber", ((object)entity.TrainNumber) ?? DBNull.Value),
                new SqlParameter("@StartStation", ((object)entity.StartStation) ?? DBNull.Value),
                new SqlParameter("@EndStation", ((object)entity.EndStation) ?? DBNull.Value),
                new SqlParameter("@GoTime", ((object)entity.GoTime) ?? DBNull.Value),
                new SqlParameter("@ComeTime", ((object)entity.ComeTime) ?? DBNull.Value),
                new SqlParameter("@TimeSpan", ((object)entity.TimeSpan) ?? DBNull.Value)
            };

            return ExecuteNonQuery(sql, ps);
        }

        public static DataTable GetAllTrainNames()
        {
            var sql = "SELECT TrainNumber FROM Train ";

            return GetDataTable(sql);
        }
    }
}
