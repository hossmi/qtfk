using QTFK.Attributes;
using QTFK.Services;

namespace QTFK.Models.DBIO
{
    [OleDB]
    public class OleDBInsertQuery : AbstractInsertQuery
    {
        public OleDBInsertQuery(IParameterBuilder parameterBuilder) : base(parameterBuilder)
        {
        }
    }
}