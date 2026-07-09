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

    // --------------- Example ---------------

    // in your domain
    public interface IDB2Context : IAbstractRepositoryContext
    {
    }

    public interface ICoreContext : IAbstractRepositoryContext
    {
    }

    public interface ICoreRepo : IAbstractRepository<ICoreContext>
    {
    }

    public interface ICoreUserRepo : ICoreRepo
    {
        Task<List<string>> GetUserAsync();
    }

    public interface ICoreProductRepo : ICoreRepo
    {
    }


    // in your infra
    public class CountingRepoContext : DapperRepositoryContextSQLBase, ICoreContext
    {
        public CountingRepoContext(string con)
            : base(con)
        {
        }
    }

    public class CoreUserRepo : DapperRepositorySQLBase<CountingRepoContext> //, ICoreUserRepo
    {
        public CoreUserRepo(CountingRepoContext context)
            : base(context)
        {
        }


        public async Task<List<string>> GetUserAsync()
        {
            var paramList = RepoContext.NewDbParams();

            paramList.Int("id", 1);
            paramList.Date("dt", DateTime.UtcNow);

            var ret = await RepoContext.QueryAsync<string>("", param: paramList.ToProviderParams());
            return ret;
        }
    }


    public class MyApp
    {
        private readonly ICoreContext Context;

        public MyApp(ICoreContext context)
        {
            Context = context;
        }

        public void DoJob()
        {
            //var repoUser = Context.GetRepository<ICoreUserRepo>();

            //repoUser.GetUserAsync().GetAwaitResult();
        }
    }
}
