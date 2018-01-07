using QTFK.Attributes;
using QTFK.Services;

namespace QTFK.Models.DBIO
{
    [SqlServer]
    internal class SqlSelectQuery : AbstractSelectQuery
    {
        public SqlSelectQuery(IParameterBuilder parameterBuilder) : base(parameterBuilder)
        {

        }
    }
}