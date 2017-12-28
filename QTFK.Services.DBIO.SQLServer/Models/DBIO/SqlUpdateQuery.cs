using QTFK.Attributes;
using QTFK.Services;

namespace QTFK.Models.DBIO
{
    [SqlServer]
    public class SqlUpdateQuery : AbstractUpdateQuery
    {
        public SqlUpdateQuery(IParameterBuilder parameterBuilder) : base(parameterBuilder)
        {
        }
    }
}