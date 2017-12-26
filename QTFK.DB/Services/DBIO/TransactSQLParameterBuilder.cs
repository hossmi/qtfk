using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services.DBIO
{
    public class TransactSQLParameterBuilder : IParameterBuilder
    {
        public string buildParameter(string parameterName)
        {
            string result;
            string parameter;
            char[] validChars;

            validChars = parameterName
                .Where(c => c > 32)
                .ToArray();

            parameter = new string(validChars);
            result = $"@{parameter}";

            return result;
        }
    }
}
