using Dapper;

namespace MOR.Infrastructure.Persistance.DapperOrm
{
    public class DapperMultiReader
    {
        private readonly SqlMapper.GridReader Reader;


        internal DapperMultiReader(SqlMapper.GridReader reader)
        {
            Reader = reader;
        }


        public async Task<List<T>> ReadAsync<T>()
        {
            var result = await Reader.ReadAsync<T>();

            var ret = result.AsList();
            return ret;
        }
    }
}
