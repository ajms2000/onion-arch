namespace MOR.Repositories
{
    public interface IRepository
    {
    }

    public interface IRepository<TUnitOfWork>
        where TUnitOfWork : IUnitOfWork
    {
    }
}
