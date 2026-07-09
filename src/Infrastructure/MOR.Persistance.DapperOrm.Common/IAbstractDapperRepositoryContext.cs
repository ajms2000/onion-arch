using MOR.Repositories;
using System.Data;

namespace MOR.Persistance.DapperOrm
{
    public interface IAbstractDapperRepositoryContext : IAbstractRepositoryContext
    {
        IDbParams NewDbParams();
    }
}
