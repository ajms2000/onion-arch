using Microsoft.Data.SqlClient;
using System.Data.Sql;

namespace MOR.Infrastructure.Persistance.DapperOrm.SQL
{
    public class DapperQueryExecutorSQL : DapperQueryExecutorBase<SqlConnection, SqlTransaction, SqlDbConnectionManager>
    {
        public DapperQueryExecutorSQL(SqlDbConnectionManager connectionManager)
            : base(connectionManager)
        {
        }
    }
}
