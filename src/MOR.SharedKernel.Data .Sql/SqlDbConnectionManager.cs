using System.Data.Common;

namespace System.Data.Sql
{
    public class SqlDbConnectionManager : IDbConnectionManager, IDisposable
    {
        private string ConnectionString = string.Empty;
        private DbConnection? m_Connection;
        private DbTransaction? m_Trans;
        private bool m_IsInTransaction = false;


        public SqlDbConnectionManager(string connectionString)
        {
            ConnectionString = connectionString;
        }


        public event EventHandler<DbTransactionStateEventArgs>? TransactionStateChanged;


        public DbConnection GetConnection(bool isOpen = false)
        {
            var con = GetConnectionCore(out bool canOpen);

            if (isOpen && canOpen)
            {
                con.Open();
            }

            return con;
        }

        public async Task<DbConnection> GetConnectionAsync(bool isOpen = false, CancellationToken cancellationToken = default)
        {
            var con = GetConnectionCore(out bool canOpen);

            if (isOpen && canOpen)
            {
                await con.OpenAsync(cancellationToken);
            }

            return con;
        }

        public void SetConnectionString(string connectionString)
        {
            if (connectionString.NullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            Dispose();

            ConnectionString = connectionString;
        }


        public DbTransaction BeginTransaction()
        {
            if (m_Trans == null)
            {
                var con = GetConnection(true);
                m_Trans = con.BeginTransaction();

                m_IsInTransaction = true;

                FileTransactionStateChanged(true);
            }

            return m_Trans;
        }

        public async Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (m_Trans == null)
            {
                var con = await GetConnectionAsync(true, cancellationToken);
                m_Trans = con.BeginTransaction();

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

        public bool TryGetCurrentTransaction(out DbTransaction? transaction)
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



        private DbConnection GetConnectionCore(out bool canOpen)
        {
            if (m_Connection == null)
            {
                m_Connection = new Microsoft.Data.SqlClient.SqlConnection(ConnectionString);
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


        ~SqlDbConnectionManager()
        {
            DisposeTransaction();
            DisposeConnection();
        }
    }
}
