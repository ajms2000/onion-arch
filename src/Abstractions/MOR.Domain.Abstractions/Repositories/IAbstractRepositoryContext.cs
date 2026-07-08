namespace MOR.Repositories
{
    public interface IAbstractRepositoryContext
    {
        TRepository GetRepository<TRepository>()
            where TRepository : IAbstractRepository<IAbstractRepositoryContext>;
    }
}
