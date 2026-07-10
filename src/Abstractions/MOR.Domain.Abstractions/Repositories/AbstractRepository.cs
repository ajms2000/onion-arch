namespace MOR.Repositories
{
    public abstract class AbstractRepository<TRepositoryContext> : IAbstractRepository<TRepositoryContext>
        where TRepositoryContext : IAbstractRepositoryContext
    {
        public AbstractRepository(TRepositoryContext context)
        {
            RepoContext = context;
        }


        public abstract TRepositoryContext RepoContext { get; set; }
    }
}
