namespace MOR.Repositories
{
    public abstract class AbstractRepository : IAbstractRepository
    {
        public AbstractRepository(IAbstractRepositoryContext repoContext)
        {
            RepoContext = repoContext;
        }


        protected virtual IAbstractRepositoryContext RepoContext { get; private set; }
    }

    public abstract class AbstractRepository<TRepositoryContext> : AbstractRepository, IAbstractRepository<TRepositoryContext>
        where TRepositoryContext : IAbstractRepositoryContext
    {
        public AbstractRepository(TRepositoryContext uow)
            : base(uow)
        {
        }
    }
}
