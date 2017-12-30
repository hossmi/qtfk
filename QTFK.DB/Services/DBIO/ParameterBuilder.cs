using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services.DBIO
{
    public abstract class ParameterBuilder : IParameterBuilder
    {
        private static IParameterBuilder tsqlParameterBuilder;

        public virtual string buildParameter(string parameterName)
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

        public static IParameterBuilder TSQL
        {
            get
            {
                if (tsqlParameterBuilder == null)
                    tsqlParameterBuilder = new PrvTransactSQLParameterBuilder();

                return tsqlParameterBuilder;
            }
        }

        private class PrvTransactSQLParameterBuilder : ParameterBuilder
        {

        }
    }
}
