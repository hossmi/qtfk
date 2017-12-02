namespace QTFK.Models
{
    public interface IByParamBetweenFilter<T> : IQueryFilter where T: struct
    {
        string Field { get; set; }
        T Min { get; set; }
        T Max { get; set; }
    }
}