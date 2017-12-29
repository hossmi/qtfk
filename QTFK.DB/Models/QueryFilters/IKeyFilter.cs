namespace QTFK.Models.QueryFilters
{
    public interface IKeyFilter : IQueryFilter
    {
        void setKey(string key, object value);
    }
}
