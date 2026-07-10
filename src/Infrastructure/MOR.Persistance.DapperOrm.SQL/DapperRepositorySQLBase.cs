using MOR.Repositories;

namespace MOR.Persistance.DapperOrm.SQL
{
    public abstract class DapperRepositorySQLBase<TRepositoryContext> : AbstractRepository<TRepositoryContext>
        where TRepositoryContext : DapperRepositoryContextSQLBase
    {
        public DapperRepositorySQLBase(TRepositoryContext context)
            : base(context)
        {
        }
    }

    public abstract class DapperRepositorySQLBase : DapperRepositorySQLBase<DapperRepositoryContextSQLBase>
    {
        public DapperRepositorySQLBase(DapperRepositoryContextSQLBase context)
            : base(context)
        {
        }
    }
}
