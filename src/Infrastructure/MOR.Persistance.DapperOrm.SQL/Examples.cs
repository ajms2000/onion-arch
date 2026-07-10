using MOR.Repositories;

namespace MOR.Persistance.DapperOrm.SQL
{
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
        Task<List<string>> GetUsersAsync();
    }

    public interface ICoreProductRepo : ICoreRepo
    {
        Task<List<string>> GetProductsAsync();
    }


    // in your infra
    public class CountingRepoContext : DapperRepositoryContextSQLBase, ICoreContext
    {
        public CountingRepoContext(string con)
            : base(con)
        {
        }

        protected override IReadOnlyDictionary<Type, Type>? RepoMappings => new Dictionary<Type, Type>
        {
            { typeof(ICoreUserRepo), typeof(CoreUserRepo) },
            { typeof(ICoreProductRepo), typeof(CoreProductRepo) }
        };
    }

    public class CoreUserRepo : DapperRepositorySQLBase, ICoreUserRepo
    {
        public CoreUserRepo(DapperRepositoryContextSQLBase context)
            : base(context)
        {
        }

        public async Task<List<string>> GetUsersAsync()
        {
            var paramList = RepoContext.NewDbParams();

            paramList.Int("id", 1);
            paramList.Date("dt", DateTime.UtcNow);

            var ret = await RepoContext.QueryAsync<string>("", param: paramList.ToProviderParams());
            return ret;
        }
    }

    public class CoreProductRepo : DapperRepositorySQLBase, ICoreProductRepo
    {
        public CoreProductRepo(DapperRepositoryContextSQLBase context)
            : base(context)
        {
        }

        public async Task<List<string>> GetProductsAsync()
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

        public async Task DoJob()
        {
            var repoUser = Context.GetRepository<ICoreUserRepo>();

            await repoUser.GetUsersAsync();
        }
    }
}
