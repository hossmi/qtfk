namespace QTFK.Models
{
    public interface IQueryFilter
    {
        string Compile();
        void SetValues(params object[] args);
    }
}