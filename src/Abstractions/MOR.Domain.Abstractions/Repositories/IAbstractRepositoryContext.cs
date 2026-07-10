namespace MOR.Repositories
{
    public interface IAbstractRepositoryContext
    {
        Task<bool> OpenConnectionAsync(CancellationToken cancellationToken = default);
        Task<bool> StartTransactionAsync(CancellationToken cancellationToken = default);
        Task EndTransactionAsync(bool commit, CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        TRepository GetRepository<TRepository>()
            where TRepository : IAbstractRepository<IAbstractRepositoryContext>;

        TRepository NewRepository<TRepository>()
            where TRepository : IAbstractRepository<IAbstractRepositoryContext>;
    }
}
