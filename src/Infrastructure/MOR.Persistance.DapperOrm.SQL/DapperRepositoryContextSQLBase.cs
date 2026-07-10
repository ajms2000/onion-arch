using Microsoft.Data.SqlClient;
using MOR.Repositories;
using System.Data;
using System.Data.Sql;

namespace MOR.Persistance.DapperOrm.SQL
{
    public abstract class DapperRepositoryContextSQLBase : DapperRepositoryContextBase<SqlConnection, SqlTransaction>, IAbstractRepositoryContext
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
