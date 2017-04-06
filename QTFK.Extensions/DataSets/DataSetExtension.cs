using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.DataSets
{
    public static class DataSetExtension
    {
        public static T Get<T>(this DataRow dr, string name)
        {
            if (dr.IsNull(name))
                return default(T);
            return (T)dr[name];
        }

        public static T Get<T>(this DataRow dr, int index)
        {
            if (dr.IsNull(index))
                return default(T);
            return (T)dr[index];
        }

        public static DataTable AsTable(this DataSet ds)
        {
            if (ds == null || ds.Tables.Count <= 0)
                return null;

            return ds.Tables[0];
        }
    }
}
