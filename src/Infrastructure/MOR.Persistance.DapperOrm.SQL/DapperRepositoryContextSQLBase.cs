using Microsoft.Data.SqlClient;

namespace MOR.Persistance.DapperOrm.SQL
{
    public abstract class DapperRepositoryContextSQLBase : DapperRepositoryContextBase<SqlConnection, SqlTransaction>
    {
        public DapperRepositoryContextSQLBase(string connectionString)
            : base(connectionString)
        {
        }


        protected override SqlConnection CreateConnectionObject()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
