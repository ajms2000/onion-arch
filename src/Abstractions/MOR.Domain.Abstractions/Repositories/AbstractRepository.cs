namespace MOR.Repositories
{
    public abstract class AbstractRepository<TRepositoryContext> : IAbstractRepository<TRepositoryContext>
        where TRepositoryContext : IAbstractRepositoryContext
    {
        public AbstractRepository(TRepositoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            RepoContext = context;
        }


        public TRepositoryContext RepoContext { get; private set; }
    }
}
