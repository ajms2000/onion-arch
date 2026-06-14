namespace MOR.App.Services.PubSub
{
    public interface IQueueService
    {
        string Instance { get; }
        string Queue { get; }

        Task<bool> ExistsAsync();
        Task SendMessageAsync(string message);
    }

    public interface IQueueServiceProvider
    {
        string Instance { get; }

        ValueTask<IQueueService> GetQueueServiceAsync(string queue);
        Task<List<string>> GetQueueNamesAsync();
    }
}
