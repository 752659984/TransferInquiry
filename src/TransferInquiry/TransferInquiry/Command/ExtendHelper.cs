using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferInquiry.Command
{
    public static class ExtendHelper
    {
        public static DataTable ListEntityToDataTable<T>(this List<T> entitys) where T : class, new()
        {
            var type = typeof(T);
            var result = new DataTable();
            var ps = type.GetProperties();

            foreach (var p in ps)
            {
                result.Columns.Add(new DataColumn(p.Name, p.PropertyType));
            }

            if (entitys == null || entitys.Count <= 0)
            {
                return result;
            }

            foreach (var entity in entitys)
            {
                var row = result.NewRow();
                foreach (var p in ps)
                {
                    object val = p.GetValue(entity);
                    if (val == null)
                    {
                        val = DBNull.Value;
                    }
                    row[p.Name] = val;
                }
                result.Rows.Add(row);
            }

            return result;
        }

        public static List<T> DataTableToList<T>(this DataTable dt) where T : class, new()
        {
            var result = new List<T>();
            if (dt == null || dt.Rows.Count <= 0)
            {
                return result;
            }
            
            var type = typeof(T);
            var ps = type.GetProperties();
            foreach (DataRow row in dt.Rows)
            {
                var t = new T();
                foreach (var p in ps)
                {
                    p.SetValue(t, Convert.ChangeType(row[p.Name], p.PropertyType));
                }
                result.Add(t);
            }

            return result;
        }
    }
}
