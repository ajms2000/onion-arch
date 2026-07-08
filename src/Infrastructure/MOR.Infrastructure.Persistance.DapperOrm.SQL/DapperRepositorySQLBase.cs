using MOR.Repositories;

namespace MOR.Infrastructure.Persistance.DapperOrm.SQL
{
    public abstract class DapperRepositorySQLBase : DapperRepositorySQLBase<DapperRepositoryContextSQL>
    {
        public DapperRepositorySQLBase(DapperRepositoryContextSQL context)
            : base(context)
        {
        }
    }

    public abstract class DapperRepositorySQLBase<TRepositoryContext> : AbstractRepository<TRepositoryContext>
        where TRepositoryContext : DapperRepositoryContextSQL
    {
        public DapperRepositorySQLBase(TRepositoryContext context)
            : base(context)
        {
        }
    }
}
