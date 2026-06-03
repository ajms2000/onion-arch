using System.Data.Common;

namespace System.Data
{
    public interface IDataRepositoryTransactionContext
    {
        void EndTransaction(bool commit);
        Task EndTransactionAsync(bool commit);
    }

    public interface IDbConnectionManager : IDataRepositoryTransactionContext
    {
        event EventHandler<DbTransactionStateEventArgs> TransactionStateChanged;

        DbConnection GetConnection(bool isOpen = false);
        Task<DbConnection> GetConnectionAsync(bool isOpen = false);

        DbTransaction BeginTransaction();
        Task<DbTransaction> BeginTransactionAsync();

        bool TryGetCurrentTransaction(out DbTransaction transaction);
        bool IsInTransaction();
    }
}
