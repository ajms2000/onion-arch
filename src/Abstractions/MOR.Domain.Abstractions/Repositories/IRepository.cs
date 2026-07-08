namespace MOR.Repositories
{
    public interface IRepository
    {
    }

    public interface IRepository<TRepositoryContext>
        where TRepositoryContext : IRepositoryContext
    {
    }
}
