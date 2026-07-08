using MOR.Repositories;
using System.Data.Sql;

namespace MOR.Infrastructure.Persistance.DapperOrm.SQL
{
    public abstract class DapperRepositoryContextSQLBase : SqlDbConnectionManager, IAbstractRepositoryContext
    {
        public DapperRepositoryContextSQLBase(string connectionString)
            : base(connectionString)
        {
        }


        public TRepository GetRepository<TRepository>()
            where TRepository : IAbstractRepository<IAbstractRepositoryContext>
        {
            throw new NotImplementedException();
        }
    }
}
