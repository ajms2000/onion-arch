using IBM.Data.Db2;

namespace MOR.Persistance.DapperOrm.DB2
{
    public class DapperRepositoryContextDB2Base : DapperRepositoryContextBase<DB2Connection, DB2Transaction>
    {
        public DapperRepositoryContextDB2Base(string connectionString)
            : base(connectionString)
        {
        }

        protected override DB2Connection CreateConnectionObject()
        {
            return new DB2Connection(ConnectionString);
        }
    }
}
