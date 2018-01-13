using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services.DBIO
{
    public class TSQLParameterBuilderFactory : IParameterBuilderFactory
    {
        public IParameterBuilder buildInstance()
        {
            return new PrvTSQLParameterBuilder();
        }

        private class PrvTSQLParameterBuilder : IParameterBuilder
        {
            private IDictionary<string, int> parameters;

            public PrvTSQLParameterBuilder()
            {
                this.parameters = new Dictionary<string, int>();
            }

            public string buildParameter(string parameterName)
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
        }
    }
}
