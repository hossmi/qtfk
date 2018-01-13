using QTFK.Attributes;
using QTFK.Services;

namespace QTFK.Models.DBIO
{
    [OleDB]
    internal class OleDBInsertQuery : AbstractInsertQuery
    {
        public OleDBInsertQuery(IParameterBuilderFactory parameterBuilderFactory) : base(parameterBuilderFactory)
        {
        }
    }
}