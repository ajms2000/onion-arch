using Microsoft.Data.SqlClient;

namespace System.Data.Sql
{
    public class SqlDbConnectionManager : DbConnectionManagerBase<SqlConnection, SqlTransaction>
    {
        public SqlDbConnectionManager(string connectionString)
            : base(connectionString)
        {
        }


        protected override SqlConnection CreateConnectionObject()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
