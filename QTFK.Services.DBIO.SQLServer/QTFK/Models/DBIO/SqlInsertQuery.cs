using QTFK.Attributes;
using QTFK.Services;

namespace QTFK.Models.DBIO
{
    [SqlServer]
    internal class SqlInsertQuery : AbstractInsertQuery
    {
        public SqlInsertQuery(IParameterBuilderFactory parameterBuilderFactory) : base(parameterBuilderFactory)
        {
        }
    }
}