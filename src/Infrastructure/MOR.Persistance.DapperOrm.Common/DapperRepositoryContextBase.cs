using Dapper;
using MOR.Repositories;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;

namespace MOR.Persistance.DapperOrm
{
    // TODO : Add polly

    public abstract class DapperRepositoryContextBase<TDbConnection, TDbTransaction> : DbConnectionManagerBase<TDbConnection, TDbTransaction>, IAbstractRepositoryContext
        where TDbConnection : DbConnection, new()
        where TDbTransaction : DbTransaction
    {
        private readonly ConcurrentDictionary<Type, IAbstractRepository<IAbstractRepositoryContext>> RepoCache = new ConcurrentDictionary<Type, IAbstractRepository<IAbstractRepositoryContext>>();


        public DapperRepositoryContextBase(string connectionString)
            : base(connectionString)
        {
        }


        // ------- IAbstractRepositoryContext Functions -------

        public async Task<bool> OpenConnectionAsync(CancellationToken cancellationToken = default)
        {
            var con = await GetConnectionAsync(isOpen: true, cancellationToken);

            var ret = con.State == ConnectionState.Open;
            return ret;
        }

        public async Task<bool> StartTransactionAsync(CancellationToken cancellationToken = default)
        {
            await BeginTransactionAsync(cancellationToken);

            var ret = IsInTransaction();
            return ret;
        }

        /// <summary>
        /// This does not have any effect in base implementation. Extend as required.
        /// </summary>
        public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(int.MinValue);
        }

        public abstract IDbParams NewDbParams();


        // ------- Repository Factory -------

        public virtual TRepository GetRepository<TRepository>()
            where TRepository : IAbstractRepository<IAbstractRepositoryContext>
        {
            var repoType = typeof(TRepository);

            var repo = RepoCache.GetOrAdd(repoType, (t) =>
            {
                return NewRepository<TRepository>();
            });

            if (repo == null)
            {
                throw new InvalidOperationException($"Unable to obtain repository of type '{repoType.FullName}'.");
            }

            return (TRepository)repo;
        }

        public abstract TRepository NewRepository<TRepository>()
            where TRepository : IAbstractRepository<IAbstractRepositoryContext>;


        // ------- Dapper Wrappers -------

        public async Task<List<T>> QueryAsync<T>(string sql, object? param = null, CommandType? commandType = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            var oParam = param ?? GenerateParameters(param!);

            var con = await base.GetConnectionAsync(cancellationToken: cancellationToken);

            base.TryGetCurrentTransaction(out TDbTransaction? trans);

            var result = await con.QueryAsync<T>(
                sql,
                param: param,
                commandType: commandType,
                transaction: trans,
                commandTimeout: timeout);

            var ret = result.AsList();
            return ret;
        }


        public async Task<DapperMultiReader> QueryMultipleAsync(string sql, object? param = null, CommandType? commandType = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            var oParam = param ?? GenerateParameters(param!);

            var con = await base.GetConnectionAsync(cancellationToken: cancellationToken);

            base.TryGetCurrentTransaction(out TDbTransaction? trans);

            var reader = await con.QueryMultipleAsync(
                sql,
                param: oParam,
                commandType: commandType,
                transaction: trans,
                commandTimeout: timeout);

            var ret = new DapperMultiReader(reader);
            return ret;
        }


        public async Task<int> ExecuteAsync(string sql, object? param = null, CommandType? commandType = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            var oParam = param ?? GenerateParameters(param!);

            var con = await base.GetConnectionAsync(cancellationToken: cancellationToken);

            base.TryGetCurrentTransaction(out TDbTransaction? trans);

            var ret = await con.ExecuteAsync(
                sql,
                param: oParam,
                commandType: commandType,
                transaction: trans,
                commandTimeout: timeout);

            return ret;
        }


        public async Task<Stream?> ReadSingleStreamAsync(string sql, object? param = null, CommandType? commandType = null, int dataColumnIndex = 0, int? timeout = null, CancellationToken cancellationToken = default)
        {
            var oParam = param ?? GenerateParameters(param!);

            var con = await base.GetConnectionAsync(cancellationToken: cancellationToken);

            base.TryGetCurrentTransaction(out TDbTransaction? trans);

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
