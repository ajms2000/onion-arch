using IBM.Data.Db2;

namespace MOR.Infrastructure.Persistance.DapperOrm.DB2
{
    public class DapperRepositoryContextDB2 : DapperRepositoryContextBase<DB2Connection, DB2Transaction>
    {
        public DapperRepositoryContextDB2(string connectionString)
            : base(connectionString)
        {
        }

        protected override DB2Connection CreateConnectionObject()
        {
            return new DB2Connection(ConnectionString);
        }
    }
}
