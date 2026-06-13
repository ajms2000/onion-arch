using Microsoft.Data.SqlClient;

namespace System.Data.Sql
{
    public class SqlSingleBinaryStreamAdapter : DbSingleBinaryStreamAdapter<SqlConnection, SqlTransaction, SqlCommand, SqlParameter>
    {
        public SqlSingleBinaryStreamAdapter(string connectionString)
            : base(connectionString)
        {
        }
    }
}
