using QTFK.Attributes;
using QTFK.Services;

namespace QTFK.Models.DBIO
{
    [SqlServer]
    internal class SqlDeleteQuery : AbstractDeleteQuery
    {
        public SqlDeleteQuery(IParameterBuilder parameterBuilder) : base(parameterBuilder)
        {

        }
    }
}