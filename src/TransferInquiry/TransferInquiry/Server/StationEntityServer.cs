using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferInquiry.Command;
using TransferInquiry.DataBase;
using TransferInquiry.Entitys;

namespace TransferInquiry.Server
{
    public class StationEntityServer : DBHelper
    {
        public static void InsertStationNames<T>(IEnumerable<T> list) where T : class, new()
        {
            TruncateTable("Station");
            BulkToDB(list.ToList().ListEntityToDataTable(), "Station");
        }

        public static List<StationEntity> GetStations()
        {
            var sql = @"SELECT [ID]
            ,[FN]
            ,[FP]
            ,[JP]
            ,[OP]
            ,[SC]
            ,[City]
              FROM [TransferInquiry].[dbo].[Station]
              where fn=city or city in
              (
              select s1.city 
              from [Station] s1
              where not exists(select 1 from [Station] s2 where s2.fn=s2.city and s1.city=s2.city)
              group by s1.city
              )
              --or city is null";

            return GetDataTable(sql).DataTableToList<StationEntity>();
        }

        public static DataTable GetTransferThree(string startName, string endName)
        {
            var sql = @"select t1.TrainNumber as 'Step1Train', t1.QueryStartCode as 'Step1StartStation', t1.QueryEndCode  as 'Step1EndStation', 
            q.TrainNumber as 'Step2Train', q.QueryStartCode as 'Step2StartStation', q.QueryEndCode  as 'Step2EndStation', 
            t2.TrainNumber as 'Step3Train', t2.QueryStartCode as 'Step3StartStation', t2.QueryEndCode  as 'Step3EndStation'
            from (select q1.* from QueryTrainResult q1 inner join Station s1 on q1.QueryStartCode=s1.SC where s1.fn like '{0}%') t1
            inner join QueryTrainResult q on t1.QueryEndCode=q.QueryStartCode
            inner join (select q2.* from QueryTrainResult q2 inner join Station s2 on q2.QueryEndCode=s2.SC where s2.fn like '{1}%') t2 on q.QueryEndCode=t2.QueryStartCode";

            sql = string.Format(sql, startName, endName);

            return GetDataTable(sql);
        }
    }
}
