using IBM.Data.Db2;
using System.Data;

namespace MOR.Persistance.DapperOrm.DB2
{
    public class DapperRepositoryContextDB2Base : DapperRepositoryContextBase<DB2Connection, DB2Transaction>
    {
        public DapperRepositoryContextDB2Base(string connectionString)
            : base(connectionString)
        {
        }


        public override IDbParams NewDbParams()
        {
            return new DB2DbParams();
        }


        protected override DB2Connection CreateConnectionObject()
        {
            return new DB2Connection(ConnectionString);
        }
    }
}
