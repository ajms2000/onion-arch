using System.Data.Common;

namespace System.Data
{
    public class DbFieldItemStreamResult
    {
        public bool Success { get; set; }
        public Stream? ContentStream { get; set; }
    }

    // https://www.codeproject.com/Articles/140713/Download-and-Upload-Images-from-SQL-Server-via-ASP

    // Solutions in Framework 4.5
    // https://stackoverflow.com/questions/37837484/uploading-stream-to-database
    // https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sqlclient-streaming-support

    public class DbSingleBinaryStreamAdapter<TConnection, TTransaction, TCommand, TParameter>
        where TConnection : DbConnection, new()
        where TCommand : DbCommand, new()
        where TTransaction : DbTransaction
        where TParameter : DbParameter
    {
        private const int UPLOAD_BUFFER_SIZE = 8040;
        private string ConnectionString;


        public DbSingleBinaryStreamAdapter(string connectionString)
        {
            if (connectionString.NullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            ConnectionString = connectionString;
        }


        /// <summary>
        /// Reads a single row field value as stream.
        /// </summary>
        /// <param name="sql">Sql statement to read the field. Must return a single row.</param>
        /// <param name="parameters">Db parameters to apply on the query. Can be null or empty.</param>
        /// <param name="streamFieldColumnIndex">If the SQL statement return multiple columns, then specify the column index of the interested field to read as stream.</param>
        /// <returns></returns>
        public async Task<DbFieldItemStreamResult> GetSingleResultAsync(string sql, IEnumerable<TParameter> parameters, int streamFieldColumnIndex = 0)
        {
            var ret = new DbFieldItemStreamResult();
            var con = NewConnection();

            try
            {
                var command = new TCommand
                {
                    Connection = con,
                    CommandText = sql,
                };

                if (parameters.AnyAndNotNull())
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }

                var reader = await command.ExecuteReaderAsync(
                    CommandBehavior.SequentialAccess |
                    CommandBehavior.SingleResult |
                    CommandBehavior.SingleRow |
                    CommandBehavior.CloseConnection);

                if (await reader.ReadAsync())
                {
                    if (!await reader.IsDBNullAsync(streamFieldColumnIndex))
                    {
                        ret.ContentStream = reader.GetStream(streamFieldColumnIndex);
                        ret.Success = true;
                    }

                    // Old Code with custom implementation
                    //ret.ContentStream = new DbFieldItemReaderStream(reader, streamFieldColumnIndex);
                    //ret.Success = true;
                }

                if (!ret.Success)
                {
                    reader.Dispose();
                }

                con = null; // ownership released
            }
            finally
            {
                if (con != null)
                {
                    con.Dispose();
                }
            }

            return ret;
        }

        /// <summary>
        /// Reads a single row field value as stream.
        /// </summary>
        /// <param name="sql">Sql statement to read the field. Must return a single row.</param>
        /// <param name="parameters">Db parameters to apply on the query. Can be null or empty.</param>
        /// <param name="streamFieldColumnIndex">If the SQL statement return multiple columns, then specify the column index of the interested field to read as stream.</param>
        /// <returns></returns>
        public DbFieldItemStreamResult GetSingleResult(string sql, IEnumerable<TParameter> parameters, int streamFieldColumnIndex = 0)
        {
            var ret = new DbFieldItemStreamResult();
            var con = NewConnection();

            try
            {
                var command = new TCommand
                {
                    Connection = con,
                    CommandText = sql,
                };

                if (parameters.AnyAndNotNull())
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }

                var reader = command.ExecuteReader(
                    CommandBehavior.SequentialAccess |
                    CommandBehavior.SingleResult |
                    CommandBehavior.SingleRow |
                    CommandBehavior.CloseConnection);

                if (reader.Read())
                {
                    if (!reader.IsDBNull(streamFieldColumnIndex))
                    {
                        ret.ContentStream = reader.GetStream(streamFieldColumnIndex);
                        ret.Success = true;
                    }

                    // Old Code with custom implementation
                    //ret.ContentStream = new DbFieldItemReaderStream(reader, streamFieldColumnIndex);
                    //ret.Success = true;
                }

                if (!ret.Success)
                {
                    reader.Dispose();
                }

                con = null; // ownership released
            }
            finally
            {
                if (con != null)
                {
                    con.Dispose();
                }
            }

            return ret;
        }


        private TConnection NewConnection(bool openConnection = true)
        {
            var con = new TConnection
            {
                ConnectionString = ConnectionString,
            };

            if (openConnection)
            {
                con.Open();
            }

            return con;
        }

        private static string ParameterizeFieldName(string field)
        {
            var ret = "@" + field.Trim().Trim('@').Trim();
            return ret;
        }
    }
}
