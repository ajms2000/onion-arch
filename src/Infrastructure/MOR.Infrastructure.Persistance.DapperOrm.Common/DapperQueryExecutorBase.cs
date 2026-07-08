using Dapper;
using System.Data;
using System.Data.Common;

namespace MOR.Infrastructure.Persistance.DapperOrm
{
    // TODO : Add polly

    public abstract class DapperQueryExecutorBase<TDbConnection, TDbTransaction, TConnectionManager>
        where TDbConnection : DbConnection, new()
        where TDbTransaction : DbTransaction
        where TConnectionManager : DbConnectionManagerBase<TDbConnection, TDbTransaction>
    {
        protected readonly TConnectionManager ConMgr;


        public DapperQueryExecutorBase(TConnectionManager connectionManager)
        {
            ConMgr = connectionManager;
        }


        public async Task<List<T>> QueryAsync<T>(string sql, object? param = null, CommandType? commandType = null, int? timeout = null)
        {
            var oParam = param ?? GenerateParameters(param!);

            var con = await ConMgr.GetConnectionAsync().ConfigureAwait(false);

            ConMgr.TryGetCurrentTransaction(out TDbTransaction? trans);

            var result = await con.QueryAsync<T>(
                sql,
                param: param,
                commandType: commandType,
                transaction: trans,
                commandTimeout: timeout).ConfigureAwait(false);

            var ret = result.AsList();
            return ret;
        }


        public async Task<DapperMultiReader> QueryMultipleAsync(string sql, object? param = null, CommandType? commandType = null, int? timeout = null)
        {
            var oParam = param ?? GenerateParameters(param!);

            var con = await ConMgr.GetConnectionAsync();

            ConMgr.TryGetCurrentTransaction(out TDbTransaction? trans);

            var reader = await con.QueryMultipleAsync(
                sql,
                param: oParam,
                commandType: commandType,
                transaction: trans,
                commandTimeout: timeout);

            var ret = new DapperMultiReader(reader);
            return ret;
        }


        public async Task<int> ExecuteAsync(string sql, object? param = null, CommandType? commandType = null, int? timeout = null)
        {
            var oParam = param ?? GenerateParameters(param!);

            var con = await ConMgr.GetConnectionAsync();

            ConMgr.TryGetCurrentTransaction(out TDbTransaction? trans);

            var ret = await con.ExecuteAsync(
                sql,
                param: oParam,
                commandType: commandType,
                transaction: trans,
                commandTimeout: timeout);

            return ret;
        }


        public async Task<Stream?> ReadSingleStreamAsync(string sql, object? param = null, CommandType? commandType = null, int dataColumnIndex = 0, int? timeout = null)
        {
            var oParam = param ?? GenerateParameters(param!);

            var con = await ConMgr.GetConnectionAsync();

            ConMgr.TryGetCurrentTransaction(out TDbTransaction? trans);

            var cDef = new CommandDefinition(sql, oParam, trans, timeout, commandType);
            var cBehav = CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow | CommandBehavior.CloseConnection;

            var reader = await con.ExecuteReaderAsync(cDef, cBehav);

            var ret = default(Stream);
            var succ = false;

            if (reader.Read())
            {
                if (!reader.IsDBNull(dataColumnIndex))
                {
                    ret = reader.GetStream(dataColumnIndex);
                    succ = true;
                }
            }

            if (!succ)
            {
                reader.Dispose();
            }

            return ret;
        }


        // ------- Privates -------

        private object GenerateParameters(object inputParams)
        {
            var ret = inputParams;

            if (inputParams != null && inputParams is IDbParams)
            {
                ret = ((IDbParams)inputParams).ToProviderParams();
            }

            return ret;
        }
    }
}
