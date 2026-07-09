using MOR.Repositories;

namespace MOR.Persistance.DapperOrm.DB2
{
    public abstract class DapperRepositoryDB2Base : DapperRepositoryDB2Base<DapperRepositoryContextDB2>
    {
        public DapperRepositoryDB2Base(DapperRepositoryContextDB2 context)
            : base(context)
        {
        }
    }

    public abstract class DapperRepositoryDB2Base<TRepositoryContext> : AbstractRepository<TRepositoryContext>
        where TRepositoryContext : DapperRepositoryContextDB2
    {
        public DapperRepositoryDB2Base(TRepositoryContext context)
            : base(context)
        {
        }
    }
}
