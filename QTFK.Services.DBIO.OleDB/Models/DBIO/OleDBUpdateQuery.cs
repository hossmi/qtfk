using QTFK.Attributes;
using QTFK.Services;

namespace QTFK.Models.DBIO
{
    [OleDB]
    public class OleDBUpdateQuery : AbstractUpdateQuery
    {
        public OleDBUpdateQuery(IParameterBuilder parameterBuilder) : base(parameterBuilder)
        {
        }
    }
}