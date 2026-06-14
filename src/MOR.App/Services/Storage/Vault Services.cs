namespace MOR.App.Services.Storage
{
    public interface IVaultService
    {
        Task<List<string>> GetSecretNamesAsync();
        Task<string> GetSecretValueAsync(string referenceOrValue);
    }
}
