using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferInquiry.DataBase;
using TransferInquiry.Entitys;

namespace TransferInquiry.Server
{
    public class TrainRouteServer:DBHelper
    {
        public static int InsertTrainRoute(TrainRoute entity)
        {
            var sql = "INSERT INTO TrainRoute(TrainNumber,StartStationName,ArriveTime,StationTrainCode,StationName,TrainClassName,ServiceType,StartTime,StopoverTime,EndStationName,StationNo,IsEnabled) ";
            sql += "VALUES(@TrainNumber,@StartStationName,@ArriveTime,@StationTrainCode,@StationName,@TrainClassName,@ServiceType,@StartTime,@StopoverTime,@EndStationName,@StationNo,@IsEnabled)";

            var ps = new SqlParameter[]
            {
                new SqlParameter("@TrainNumber", ((object)entity.TrainNumber) ?? DBNull.Value),
                new SqlParameter("@StartStationName", ((object)entity.StartStationName) ?? DBNull.Value),
                new SqlParameter("@ArriveTime", ((object)entity.ArriveTime) ?? DBNull.Value),
                new SqlParameter("@StationTrainCode", ((object)entity.StationTrainCode) ?? DBNull.Value),
                new SqlParameter("@StationName", ((object)entity.StationName) ?? DBNull.Value),
                new SqlParameter("@TrainClassName", ((object)entity.TrainClassName) ?? DBNull.Value),
                new SqlParameter("@ServiceType", ((object)entity.ServiceType) ?? DBNull.Value),
                new SqlParameter("@StartTime", ((object)entity.StartTime) ?? DBNull.Value),
                new SqlParameter("@StopoverTime", ((object)entity.StopoverTime) ?? DBNull.Value),
                new SqlParameter("@EndStationName", ((object)entity.EndStationName) ?? DBNull.Value),
                new SqlParameter("@StationNo", ((object)entity.StationNo) ?? DBNull.Value),
                new SqlParameter("@IsEnabled", ((object)entity.IsEnabled) ?? DBNull.Value)
            };

            return ExecuteNonQuery(sql, ps);
        }
    }
}
