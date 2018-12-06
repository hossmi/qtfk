using QTFK.Attributes;
using QTFK.Services;

namespace QTFK.Models.DBIO
{
    [OleDB]
    internal class OleDBUpdateQuery : AbstractUpdateQuery
    {
        public OleDBUpdateQuery(IParameterBuilderFactory parameterBuilderFactory) : base(parameterBuilderFactory)
        {
        }
    }
}