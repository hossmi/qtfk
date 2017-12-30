using QTFK.Attributes;
using QTFK.Services;

namespace QTFK.Models.DBIO
{
    [SqlServer]
    internal class SqlInsertQuery : AbstractInsertQuery
    {
        public SqlInsertQuery(IParameterBuilder parameterBuilder) : base(parameterBuilder)
        {
        }
    }
}