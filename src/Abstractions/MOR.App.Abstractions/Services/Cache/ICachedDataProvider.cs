namespace MOR.App.Services.Cache
{
    public enum CacheType
    {
        Local,
        Distributed,
    }

    public interface ICachedDataProvider
    {
        CacheType Type { get; }

        bool ContainsKey(string key);
        Task<bool> ContainsKeyAsync(string key);

        Task<T> GetOrSetObjectAsync<T>(string key, Func<Task<T>> dataProvider);
        Task RemoveAsync(string key);
    }
}
