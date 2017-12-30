using QTFK.Attributes;
using QTFK.Services;

namespace QTFK.Models.DBIO
{
    [SqlServer]
    internal class SqlUpdateQuery : AbstractUpdateQuery
    {
        public SqlUpdateQuery(IParameterBuilder parameterBuilder) : base(parameterBuilder)
        {
        }
    }
}