using QTFK.Attributes;
using QTFK.Services;

namespace QTFK.Models.DBIO
{
    [SqlServer]
    public class SqlInsertQuery : AbstractInsertQuery
    {
        public SqlInsertQuery(IParameterBuilder parameterBuilder) : base(parameterBuilder)
        {
        }
    }
}