namespace MOR.Repositories
{
    public abstract class BaseRepository : IRepository
    {
        public BaseRepository(IUnitOfWork uow)
        {
            UOW = uow;
        }


        protected virtual IUnitOfWork UOW { get; private set; }
    }

    public abstract class BaseRepository<TUnitOfWork> : BaseRepository, IRepository<TUnitOfWork>
        where TUnitOfWork : IUnitOfWork
    {
        public BaseRepository(TUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
