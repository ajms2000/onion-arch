using Microsoft.Data.SqlClient;

namespace System.Data.Sql
{
    public class SqlDbParams : DbParamsBase<SqlParameter>
    {
        protected override SqlParameter NewStoreParam(string parameterName, object? value)
        {
            var ret = new SqlParameter(parameterName, value ?? DBNull.Value);
            return ret;
        }
    }
}
