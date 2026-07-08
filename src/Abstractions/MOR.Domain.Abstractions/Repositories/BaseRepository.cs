namespace MOR.Repositories
{
    public abstract class BaseRepository : IRepository
    {
        public BaseRepository(IRepositoryContext repoContext)
        {
            RepoContext = repoContext;
        }


        protected virtual IRepositoryContext RepoContext { get; private set; }
    }

    public abstract class BaseRepository<TRepositoryContext> : BaseRepository, IRepository<TRepositoryContext>
        where TRepositoryContext : IRepositoryContext
    {
        public BaseRepository(TRepositoryContext uow)
            : base(uow)
        {
        }
    }
}
