using IBM.Data.Db2;
using System.Data;

namespace MOR.Persistance.DapperOrm.DB2
{
    internal class DB2DbParams : DbParamsBase<DB2Parameter>
    {
        protected override DB2Parameter NewStoreParam(string parameterName, object? value)
        {
            var ret = new DB2Parameter(parameterName, value ?? DBNull.Value);
            return ret;
        }
    }
}
