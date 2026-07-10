namespace MOR.Repositories
{
    public interface IAbstractRepository<out TRepositoryContext>
        where TRepositoryContext : IAbstractRepositoryContext
    {
    }
}
