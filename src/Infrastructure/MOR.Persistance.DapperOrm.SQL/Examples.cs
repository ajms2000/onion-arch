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

        public override TRepository NewRepository<TRepository>()
        {
            var repoType = typeof(TRepository);
            var ret = default(DapperRepositorySQLBase);

            if (repoType == typeof(ICoreUserRepo))
            {
                ret = new CoreUserRepo(this);
            }

            return (TRepository)(object)ret!;
        }
    }

    public class CoreUserRepo : DapperRepositorySQLBase, ICoreUserRepo
    {
        public CoreUserRepo(DapperRepositoryContextSQLBase context)
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

        public async Task DoJob()
        {
            var repoUser = Context.GetRepository<ICoreUserRepo>();

            await repoUser.GetUserAsync();
        }
    }
}
