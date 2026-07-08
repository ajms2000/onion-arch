using System.Data.Common;

namespace System.Data
{
    public abstract class DbConnectionManagerBase<TDbConnection, TDbTransaction> : IDisposable
        where TDbConnection : DbConnection, new()
        where TDbTransaction : DbTransaction
    {
        private TDbConnection? m_Connection;
        private TDbTransaction? m_Trans;
        private bool m_IsInTransaction = false;


        public DbConnectionManagerBase(string connectionString)
        {
            if (connectionString.NullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            ConnectionString = connectionString;
        }


        public event EventHandler<DbTransactionStateEventArgs>? TransactionStateChanged;


        protected string ConnectionString { get; private set; }


        public TDbConnection GetConnection(bool isOpen = false)
        {
            var con = GetConnectionCore(out bool canOpen);

            if (isOpen && canOpen)
            {
                con.Open();
            }

            return con;
        }

        public async ValueTask<TDbConnection> GetConnectionAsync(bool isOpen = false, CancellationToken cancellationToken = default)
        {
            var con = GetConnectionCore(out bool canOpen);

            if (isOpen && canOpen)
            {
                await con.OpenAsync(cancellationToken);
            }

            return con;
        }

        //public void SetConnectionString(string connectionString)
        //{
        //    if (connectionString.NullOrWhiteSpace())
        //    {
        //        throw new ArgumentNullException(nameof(connectionString));
        //    }

        //    Dispose();

        //    ConnectionString = connectionString;
        //}


        public TDbTransaction BeginTransaction()
        {
            if (m_Trans == null)
            {
                var con = GetConnection(true);
                m_Trans = (TDbTransaction)con.BeginTransaction();

                m_IsInTransaction = true;

                FileTransactionStateChanged(true);
            }

            return m_Trans;
        }

        public async Task<TDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (m_Trans == null)
            {
                var con = await GetConnectionAsync(true, cancellationToken);
                m_Trans = (TDbTransaction)con.BeginTransaction();

                m_IsInTransaction = true;

                FileTransactionStateChanged(true);
            }

            return m_Trans;
        }

        public void EndTransaction(bool commit)
        {
            if (m_Trans != null)
            {
                try
                {
                    if (commit)
                    {
                        m_Trans.Commit();
                    }
                    else
                    {
                        m_Trans.Rollback();
                    }
                }
                finally
                {
                    DisposeTransaction();
                    DisposeConnection();

                    FileTransactionStateChanged(false);
                }
            }
        }

        public async Task EndTransactionAsync(bool commit, CancellationToken cancellationToken = default)
        {
            if (m_Trans != null)
            {
                try
                {
                    if (commit)
                    {
                        await m_Trans.CommitAsync(cancellationToken);
                    }
                    else
                    {
                        await m_Trans.RollbackAsync(cancellationToken);
                    }
                }
                finally
                {
                    DisposeTransaction();
                    DisposeConnection();

                    FileTransactionStateChanged(false);
                }
            }
        }

        public bool TryGetCurrentTransaction(out TDbTransaction? transaction)
        {
            transaction = null;

            if (m_IsInTransaction == true && m_Trans != null)
            {
                transaction = m_Trans;
                return true;
            }

            return false;
        }

        public bool IsInTransaction()
        {
            return m_IsInTransaction == true && m_Trans != null;
        }


        public void Dispose()
        {
            DisposeTransaction();
            DisposeConnection();
        }


        private void DisposeConnection()
        {
            if (m_Connection != null)
            {
                try
                {
                    m_Connection.Close();
                }
                catch
                {
                    // Ignore null contexts
                }
                finally
                {
                    m_Connection = null;
                }
            }
        }

        private void DisposeTransaction()
        {
            m_IsInTransaction = false;

            if (m_Trans != null)
            {
                try
                {
                    m_Trans.Dispose();
                }
                catch
                {
                    // Ignore null contexts
                }
                finally
                {
                    m_Trans = null;
                }
            }
        }


        protected abstract TDbConnection CreateConnectionObject();


        private TDbConnection GetConnectionCore(out bool canOpen)
        {
            if (m_Connection == null)
            {
                m_Connection = CreateConnectionObject();
            }

            canOpen =
                m_Connection.State != System.Data.ConnectionState.Open &&
                m_Connection.State != System.Data.ConnectionState.Connecting;

            return m_Connection;
        }

        private void FileTransactionStateChanged(bool isStarted)
        {
            TransactionStateChanged?.Invoke(this, new DbTransactionStateEventArgs(isStarted));
        }


        ~DbConnectionManagerBase()
        {
            DisposeTransaction();
            DisposeConnection();
        }
    }
}
