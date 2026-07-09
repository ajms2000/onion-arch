namespace MOR.Repositories
{
    public interface IAbstractRepository<TRepositoryContext>
        where TRepositoryContext : IAbstractRepositoryContext
    {
        TRepositoryContext RepoContext { get; }
    }
}
