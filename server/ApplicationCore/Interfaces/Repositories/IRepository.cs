namespace server.ApplicationCore.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        List<T> GetList();
        T GetItem(int id);
        bool Create(T item);
        bool Update(T item);
        bool Delete(int id);
    }
}
