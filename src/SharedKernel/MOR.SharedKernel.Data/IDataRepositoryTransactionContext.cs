using System.Data.Common;

namespace System.Data
{
    public interface IDataRepositoryTransactionContext
    {
        void EndTransaction(bool commit);
        Task EndTransactionAsync(bool commit, CancellationToken cancellationToken = default);
    }
}
