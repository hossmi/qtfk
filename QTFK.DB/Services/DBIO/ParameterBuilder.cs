using System.Collections.Generic;
using System.Linq;

namespace QTFK.Services.DBIO
{
    public abstract class ParameterBuilder : IParameterBuilder
    {
        protected IDictionary<string, int> parameters;

        private static IParameterBuilder tsqlParameterBuilder;

        public ParameterBuilder()
        {
            this.parameters = new Dictionary<string, int>();
        }

        public virtual string buildParameter(string parameterName)
        {
            string result;
            string parameter;
            char[] validChars;
            int number;

            validChars = parameterName
                .Where(c => c > 32)
                .ToArray();

            parameter = new string(validChars);

            number = (this.parameters.ContainsKey(parameter) ? this.parameters[parameter] : 0) + 1;
            result = $"@{parameter}{number}";
            this.parameters[parameter] = number;

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
