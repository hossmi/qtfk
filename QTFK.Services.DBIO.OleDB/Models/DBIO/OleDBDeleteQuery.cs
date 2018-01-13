using QTFK.Attributes;
using QTFK.Services;

namespace QTFK.Models.DBIO
{
    [OleDB]
    internal class OleDBDeleteQuery : AbstractDeleteQuery
    {
        public OleDBDeleteQuery(IParameterBuilderFactory parameterBuilderFactory) : base(parameterBuilderFactory)
        {
        }
    }
}