namespace QTFK.Services
{
    public interface IFactory<T>
    {
        T Build();
    }
}