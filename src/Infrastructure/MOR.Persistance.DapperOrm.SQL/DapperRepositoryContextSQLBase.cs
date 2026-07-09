using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Sql;

namespace MOR.Persistance.DapperOrm.SQL
{
    public abstract class DapperRepositoryContextSQLBase : DapperRepositoryContextBase<SqlConnection, SqlTransaction>
    {
        public DapperRepositoryContextSQLBase(string connectionString)
            : base(connectionString)
        {
        }


        public override IDbParams NewDbParams()
        {
            return new SqlDbParams();
        }


        protected override SqlConnection CreateConnectionObject()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
