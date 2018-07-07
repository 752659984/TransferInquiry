using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferInquiry.Entitys;
using TransferInquiry.Command;
using System.Data;

namespace TransferInquiry.DataBase
{
    public class DBServer : DBHelper
    {
        public static List<T> QueryDataTable<T>(string tableName) where T : class, new()
        {
            var sql = "SELECT * FROM " + tableName;
            var dt = DBHelper.GetDataTable(sql);
            return dt.DataTableToList<T>();
        }

        public static DataTable QueryDataTable(string sql, params SqlParameter[] parames)
        {
            return DBHelper.GetDataTable(sql, parames);
        }

        public static List<T> QueryDataTable<T>(string sql, params SqlParameter[] parames) where T : class, new()
        {
            var dt = DBHelper.GetDataTable(sql, parames);
            return dt.DataTableToList<T>();
        }
    }
}
