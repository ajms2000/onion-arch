namespace MOR.Repositories
{
    public abstract class BaseRepository : IRepository
    {
        public BaseRepository(IRepositoryContext uow)
        {
            UOW = uow;
        }


        protected virtual IRepositoryContext UOW { get; private set; }
    }

    public abstract class BaseRepository<TUnitOfWork> : BaseRepository, IRepository<TUnitOfWork>
        where TUnitOfWork : IRepositoryContext
    {
        public BaseRepository(TUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
