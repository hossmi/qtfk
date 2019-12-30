using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace QTFK.Extensions.DataSets
{
    public static class DataSetExtension
    {
        public static T get<T>(this DataRow dr, string name)
        {
            if (dr.IsNull(name))
                return default(T);
            return (T)dr[name];
        }

        public static T get<T>(this DataRow dr, int index)
        {
            if (dr.IsNull(index))
                return default(T);
            return (T)dr[index];
        }

        public static IEnumerable<DataColumn> getColumns(this DataTable table)
        {
            var e = table.Columns.GetEnumerator();
            while (e.MoveNext())
                yield return e.Current as DataColumn;
        }

        public static IEnumerable<DataColumn> getColumns(this DataRow row)
        {
            return getColumns(row.Table);
        }

        public static IDictionary<string, object> toDictionary(this DataRow row)
        {
            return row
                .getColumns()
                .Select(c => c.ColumnName)
                .ToDictionary(k => k, k => row.IsNull(k) ? null : row[k]);
        }
    }
}
