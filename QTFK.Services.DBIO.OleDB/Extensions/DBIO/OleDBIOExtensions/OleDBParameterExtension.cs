using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Extensions.DataReader;

namespace QTFK.Extensions.DBIO.OleDBIOExtensions
{
    public static class OleDBParameterExtension
    {
        public static ICollection<KeyValuePair<string, object>> pushDateTime(this ICollection<KeyValuePair<string, object>> parameters, string dateField, string timeField, DateTime value)
        {
            KeyValuePair<string, object> pair;

            pair = new KeyValuePair<string, object>(dateField, value.Date);
            parameters.Add(pair);
            pair = new KeyValuePair<string, object>(timeField, value.ToLongTimeString());
            parameters.Add(pair);

            return parameters;
        }

        public static ICollection<KeyValuePair<string, object>> pushDateTime(this ICollection<KeyValuePair<string, object>> parameters, string dateField, string timeField, DateTime? value)
        {
            KeyValuePair<string, object> pair;

            pair = new KeyValuePair<string, object>(dateField, value?.Date ?? null);
            parameters.Add(pair);
            pair = new KeyValuePair<string, object>(timeField, value?.ToLongTimeString() ?? null);
            parameters.Add(pair);

            return parameters;
        }

        public static DateTime getDateTime(this IDataRecord dr, string dateField, string timeField)
        {
            var dateValue = dr.Get<DateTime>(dateField).Date;
            var timeValue = dr.Get<DateTime>(timeField).TimeOfDay;

            dateValue += timeValue;
            return dateValue;
        }

        public static DateTime? getNullableDateTime(this IDataRecord dr, string dateField, string timeField)
        {
            DateTime? dateValue = dr.Get<DateTime?>(dateField)?.Date;
            TimeSpan? timeValue = dr.Get<DateTime?>(timeField)?.TimeOfDay;

            dateValue += timeValue;
            return dateValue;
        }


    }
}
