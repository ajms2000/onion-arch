using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace System.Data.Sql
{
    public static class SqlDbAdapter
    {
        private const string SANITIZE_QUOTE = "(?<!')'(?!')";

        private const string SQL_COL_COLLATION =
            "SELECT\r\n" +
            "T.name AS 'TableName',\r\n" +
            "C.name AS 'ColumnName',\r\n" +
            "C.collation_name AS 'Collation'\r\n" +
            "FROM sys.columns C\r\n" +
            "INNER JOIN sys.tables T ON T.object_id = C.object_id\r\n" +
            "WHERE T.name = '{0}'";


        public static SqlConnection CreateDatabaseConnection(string connectionString, bool isOpen = true)
        {
            var cb = new SqlConnectionStringBuilder(connectionString);
            var ret = new SqlConnection(cb.ConnectionString);

            if (isOpen)
            {
                ret.Open();
            }

            return ret;
        }

        public async static Task<SqlConnection> CreateDatabaseConnectionAsync(string connectionString, bool isOpen = true)
        {
            var cb = new SqlConnectionStringBuilder(connectionString);
            var ret = new SqlConnection(cb.ConnectionString);

            if (isOpen)
            {
                await ret.OpenAsync();
            }

            return ret;
        }

        public static SqlConnection CreateConfigDatabaseConnection(string connectionStringConfigName, bool isOpen = true)
        {
            var conStr = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringConfigName].ConnectionString;

            var ret = CreateDatabaseConnection(conStr, isOpen);
            return ret;
        }

        public static Task<SqlConnection> CreateConfigDatabaseConnectionAsync(string connectionStringConfigName, bool isOpen = true)
        {
            var conStr = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringConfigName].ConnectionString;

            return CreateDatabaseConnectionAsync(conStr, isOpen);
        }


        public static DataTable ReadData(string query, SqlConnection con, int? commandTimeout = null)
        {
            var ret = ReadData(query, con, null, commandTimeout);
            return ret;
        }

        public static DataTable ReadData(string query, SqlConnection con, SqlTransaction transaction,
            int? commandTimeout = null, IDictionary<string, object> parameters = null)
        {
            DataTable dt = new DataTable();

            using (var sqlCommand = GenerateCommand(query, con, transaction, commandTimeout, parameters))
            {
                if (commandTimeout.HasValue)
                {
                    sqlCommand.CommandTimeout = commandTimeout.Value;
                }

                using (var reader = sqlCommand.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }

            return dt;
        }

        public static Task<DataTable> ReadDataAsync(string query, SqlConnection con, int? commandTimeout = null)
        {
            return ReadDataAsync(query, con, null, commandTimeout);
        }

        public async static Task<DataTable> ReadDataAsync(string query, SqlConnection con, SqlTransaction transaction,
            int? commandTimeout = null, IDictionary<string, object> parameters = null)
        {
            DataTable dt = new DataTable();

            using (var sqlCommand = GenerateCommand(query, con, transaction, commandTimeout, parameters))
            {
                if (commandTimeout.HasValue)
                {
                    sqlCommand.CommandTimeout = commandTimeout.Value;
                }

                using (var reader = await sqlCommand.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }

            return dt;
        }


        public static object ReadSingleData(string query, SqlConnection con, int? commandTimeout = null)
        {
            var ret = ReadSingleData(query, con, null, commandTimeout);
            return ret;
        }

        public static object ReadSingleData(string query, SqlConnection con, SqlTransaction transaction,
            int? commandTimeout = null, IDictionary<string, object> parameters = null)
        {
            object ret = null;

            using (var sqlCommand = GenerateCommand(query, con, transaction, commandTimeout, parameters))
            {
                if (commandTimeout.HasValue)
                {
                    sqlCommand.CommandTimeout = commandTimeout.Value;
                }

                var obj = sqlCommand.ExecuteScalar();

                if (obj == DBNull.Value)
                {
                    ret = null;
                }
                else
                {
                    ret = obj;
                }
            }

            return ret;
        }

        public static Task<object> ReadSingleDataAsync(string query, SqlConnection con, int? commandTimeout = null)
        {
            return ReadSingleDataAsync(query, con, null, commandTimeout);
        }

        public async static Task<object> ReadSingleDataAsync(string query, SqlConnection con, SqlTransaction transaction,
            int? commandTimeout = null, IDictionary<string, object> parameters = null)
        {
            object ret = null;

            using (var sqlCommand = GenerateCommand(query, con, transaction, commandTimeout, parameters))
            {
                if (commandTimeout.HasValue)
                {
                    sqlCommand.CommandTimeout = commandTimeout.Value;
                }

                var obj = await sqlCommand.ExecuteScalarAsync();

                if (obj == DBNull.Value)
                {
                    ret = null;
                }
                else
                {
                    ret = obj;
                }
            }

            return ret;
        }


        public static Stream ReadStream(string query, SqlConnection con, int dataColumnIndex = 0, int? commandTimeout = null)
        {
            var ret = ReadStream(query, con, null, dataColumnIndex, commandTimeout);
            return ret;
        }

        public static Stream ReadStream(string query, SqlConnection con, SqlTransaction transaction, int dataColumnIndex = 0,
            int? commandTimeout = null, IDictionary<string, object> parameters = null)
        {
            using (var sqlCommand = GenerateCommand(query, con, transaction, commandTimeout, parameters))
            {
                if (commandTimeout.HasValue)
                {
                    sqlCommand.CommandTimeout = commandTimeout.Value;
                }

                using (var reader = sqlCommand.ExecuteReader())
                {
                    var ret = reader.GetStream(dataColumnIndex);
                    return ret;
                }
            }
        }

        public static Task<Stream> ReadStreamAsync(string query, SqlConnection con, int dataColumnIndex = 0, int? commandTimeout = null)
        {
            return ReadStreamAsync(query, con, null, dataColumnIndex, commandTimeout);
        }

        public async static Task<Stream> ReadStreamAsync(string query, SqlConnection con, SqlTransaction transaction, int dataColumnIndex = 0,
            int? commandTimeout = null, IDictionary<string, object> parameters = null)
        {
            using (var sqlCommand = GenerateCommand(query, con, transaction, commandTimeout, parameters))
            {
                if (commandTimeout.HasValue)
                {
                    sqlCommand.CommandTimeout = commandTimeout.Value;
                }

                using (var reader = await sqlCommand.ExecuteReaderAsync())
                {
                    var ret = reader.GetStream(dataColumnIndex);
                    return ret;
                }
            }
        }


        public static int ExecuteNonQuery(string query, SqlConnection con, int? commandTimeout = null)
        {
            var ret = ExecuteNonQuery(query, con, null, commandTimeout);
            return ret;
        }

        public static int ExecuteNonQuery(string query, SqlConnection con, SqlTransaction transaction,
            int? commandTimeout = null, IDictionary<string, object> parameters = null)
        {
            var ret = int.MinValue;

            using (var sqlCommand = GenerateCommand(query, con, transaction, commandTimeout, parameters))
            {
                if (commandTimeout.HasValue)
                {
                    sqlCommand.CommandTimeout = commandTimeout.Value;
                }

                ret = sqlCommand.ExecuteNonQuery();
            }

            return ret;
        }

        public static Task<int> ExecuteNonQueryAsync(string query, SqlConnection con, int? commandTimeout = null)
        {
            return ExecuteNonQueryAsync(query, con, null, commandTimeout);
        }

        public async static Task<int> ExecuteNonQueryAsync(string query, SqlConnection con, SqlTransaction transaction,
            int? commandTimeout = null, IDictionary<string, object> parameters = null)
        {
            var ret = int.MinValue;

            using (var sqlCommand = GenerateCommand(query, con, transaction, commandTimeout, parameters))
            {
                if (commandTimeout.HasValue)
                {
                    sqlCommand.CommandTimeout = commandTimeout.Value;
                }

                ret = await sqlCommand.ExecuteNonQueryAsync();
            }

            return ret;
        }


        public static DBColumnCollationInfo[] GetColumnCollation(string table, SqlConnection con, int? commandTimeout = null)
        {
            var ret = new List<DBColumnCollationInfo>();
            var query = string.Format(SQL_COL_COLLATION, table);
            var dt = ReadData(query, con, commandTimeout);

            foreach (DataRow dr in dt.Rows)
            {
                var cci = new DBColumnCollationInfo();
                cci.TableName = (string)dr["TableName"];
                cci.ColumnName = (string)dr["ColumnName"];
                cci.Collation = dr["Collation"] as string;

                ret.Add(cci);
            }

            return ret.ToArray();
        }

        public static Dictionary<string, DBColumnCollationInfo> GetColumnCollationLookup(string table, SqlConnection con, int? commandTimeout = null)
        {
            var list = GetColumnCollation(table, con, commandTimeout);
            var ret = list.ToDictionary(t => t.ColumnName, StringComparer.OrdinalIgnoreCase);
            return ret;
        }


        public static SqlCommand GenerateCommand(string query, SqlConnection con, SqlTransaction transaction = null, int? commandTimeout = null, IDictionary<string, object> parameters = null)
        {
            var ret = new SqlCommand(query, con, transaction) { CommandType = CommandType.Text };

            if (commandTimeout.HasValue)
            {
                ret.CommandTimeout = commandTimeout.Value;
            }

            if (parameters.AnyAndNotNull())
            {
                foreach (var kvp in parameters)
                {
                    ret.Parameters.AddWithValue(kvp.Key, kvp.Value);
                }
            }

            return ret;
        }


        public static string GetString(object value, bool forceConversion = false)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }
            else if (!forceConversion)
            {
                return (string)value;
            }
            else
            {
                return value.ToString();
            }
        }

        public static string SanitizeData(string data)
        {
            var ret = Regex.Replace(data, SANITIZE_QUOTE, "''");
            return ret;
        }
    }
}
