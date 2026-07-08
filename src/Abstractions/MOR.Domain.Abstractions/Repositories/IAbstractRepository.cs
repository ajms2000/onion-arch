namespace MOR.Repositories
{
    public interface IAbstractRepository
    {
    }

    public interface IAbstractRepository<TRepositoryContext> : IAbstractRepository
        where TRepositoryContext : IAbstractRepositoryContext
    {
    }
}
