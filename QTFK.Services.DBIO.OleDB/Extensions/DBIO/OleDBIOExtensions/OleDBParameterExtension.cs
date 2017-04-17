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
        public static IDictionary<string, object> SetDateTime(this IDictionary<string, object> parameters, string dateField, string timeField, DateTime value)
        {
            parameters[dateField] = value.Date;
            parameters[timeField] = value.ToLongTimeString();
            return parameters;
        }

        public static IDictionary<string, object> SetDateTime(this IDictionary<string, object> parameters, string dateField, string timeField, DateTime? value)
        {
            parameters[dateField] = value?.Date ?? null;
            parameters[timeField] = value?.ToLongTimeString() ?? null;
            return parameters;
        }

        public static DateTime GetDateTime(this IDataRecord dr, string dateField, string timeField)
        {
            var dateValue = dr.Get<DateTime>(dateField).Date;
            var timeValue = dr.Get<DateTime>(timeField).TimeOfDay;

            dateValue += timeValue;
            return dateValue;
        }

        public static DateTime? GetNullableDateTime(this IDataRecord dr, string dateField, string timeField)
        {
            DateTime? dateValue = dr.Get<DateTime?>(dateField)?.Date;
            TimeSpan? timeValue = dr.Get<DateTime?>(timeField)?.TimeOfDay;

            dateValue += timeValue;
            return dateValue;
        }


    }
}
