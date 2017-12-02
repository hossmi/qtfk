namespace QTFK.Models
{
    public interface IDBQueryFilterable : IDBQuery
    {

        IQueryFilter Filter { get; set; }
    }
}