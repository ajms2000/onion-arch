namespace MOR.Repositories
{
    public abstract class AbstractRepository : IAbstractRepository
    {
        public AbstractRepository(IAbstractRepositoryContext context)
        {
            RepoContext = context;
        }


        protected virtual IAbstractRepositoryContext RepoContext { get; private set; }
    }

    public abstract class AbstractRepository<TRepositoryContext> : AbstractRepository, IAbstractRepository<TRepositoryContext>
        where TRepositoryContext : IAbstractRepositoryContext
    {
        public AbstractRepository(TRepositoryContext context)
            : base(context)
        {
        }
    }
}
