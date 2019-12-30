using System;
using System.Collections.Generic;
using System.Data;

namespace QTFK.Extensions.DataReader
{
    public static class DataReaderExtension
    {
        public static T get<T>(this IDataRecord dr, string name)
        {
            object value = dr[name];
            if (DBNull.Value == value)
                return default(T);
            return (T)value;
        }

        public static T get<T>(this IDataRecord dr, int index)
        {
            if (dr.IsDBNull(index))
                return default(T);
            return (T)dr[index];
        }
    }
}
