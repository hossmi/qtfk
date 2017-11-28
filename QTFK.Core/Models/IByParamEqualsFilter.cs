namespace QTFK.Models
{
    public interface IByParamEqualsFilter : IQueryFilter
    {
        string Field { get; set; }
        string Parameter { get; set; }
    }
}