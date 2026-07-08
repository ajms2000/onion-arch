using Microsoft.Data.SqlClient;

namespace MOR.Infrastructure.Persistance.DapperOrm.SQL
{
    public class DapperRepositoryContextSQL : DapperRepositoryContextBase<SqlConnection, SqlTransaction>
    {
        public DapperRepositoryContextSQL(string connectionString)
            : base(connectionString)
        {
        }

        protected override SqlConnection CreateConnectionObject()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
