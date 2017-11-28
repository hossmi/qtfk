using QTFK.Models;

namespace QTFK.Services
{
    public interface IQueryFilterFactory { }

    public interface IByParamEqualsFilterFactory : IQueryFilterFactory
    {
        IByParamEqualsFilter NewByParamEqualsFilter();
    }

    public interface IByParamBetweenFilterFactory : IQueryFilterFactory
    {
        IByParamBetweenFilter<T> NewByParamBetweenFilter<T>() where T : struct;
    }
}
