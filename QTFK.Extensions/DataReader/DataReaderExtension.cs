using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.DataReader
{
    public static class DataReaderExtension
    {
        public static T Get<T>(this IDataRecord dr, string name)
        {
            object value = dr[name];
            if (DBNull.Value == value)
                return default(T);
            return (T)value;
        }

        public static T Get<T>(this IDataRecord dr, int index)
        {
            if (dr.IsDBNull(index))
                return default(T);
            return (T)dr[index];
        }

        public static bool IsDBNull(this IDataRecord dr, string name)
        {
            return dr.IsDBNull(dr.GetOrdinal(name));
        }

        public static IEnumerable<IDataRecord> GetRecords(this IDataReader dr)
        {
            while (dr.Read())
                yield return dr;

            if (!dr.IsClosed)
                dr.Close();
        }
    }
}
