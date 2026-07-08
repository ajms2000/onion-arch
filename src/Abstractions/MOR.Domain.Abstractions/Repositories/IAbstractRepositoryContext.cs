using System.Data.Common;

namespace MOR.Repositories
{
    public interface IAbstractRepositoryContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IAbstractRepositoryContext<TDbConnection, TDbTransaction> : IAbstractRepositoryContext
        where TDbConnection : DbConnection
        where TDbTransaction : DbTransaction
    {
        ValueTask<TDbConnection> GetConnectionAsync(bool isOpen = false, CancellationToken cancellationToken = default);
        Task<TDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task EndTransactionAsync(bool commit, CancellationToken cancellationToken = default);
    }
}
