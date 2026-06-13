using System.Data.Common;

namespace System.Data
{
    public interface IDataRepositoryTransactionContext
    {
        void EndTransaction(bool commit);
        Task EndTransactionAsync(bool commit, CancellationToken cancellationToken = default);
    }

    public interface IDbConnectionManager : IDataRepositoryTransactionContext
    {
        event EventHandler<DbTransactionStateEventArgs> TransactionStateChanged;

        DbConnection GetConnection(bool isOpen = false);
        Task<DbConnection> GetConnectionAsync(bool isOpen = false, CancellationToken cancellationToken = default);

        DbTransaction BeginTransaction();
        Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        bool TryGetCurrentTransaction(out DbTransaction? transaction);
        bool IsInTransaction();
    }
}
