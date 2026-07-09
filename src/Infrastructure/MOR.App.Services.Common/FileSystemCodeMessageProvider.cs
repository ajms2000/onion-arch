using MOR.Services;

namespace MOR.App.Services.Common
{
    public class FileSystemCodeMessageProvider : ICodeMessageProvider
    {
        private readonly Dictionary<string, string> Lookup;


        public FileSystemCodeMessageProvider(params string[] filenames)
        {
            Lookup = LoadAllMessagesAsync(filenames).GetAwaitResult();
        }


        public ValueTask<string?> GetMessageAsync(string code, string? defaultValue = null)
        {
            if (!Lookup.TryGetValue(code, out var ret))
            {
                ret = defaultValue;
            }

            return ValueTask.FromResult(ret);
        }

        public string? GetMessage(string code, string? defaultValue = null)
        {
            if (!Lookup.TryGetValue(code, out var ret))
            {
                ret = defaultValue;
            }

            return ret;
        }


        private async Task<Dictionary<string, string>> LoadAllMessagesAsync(string[] filenames)
        {
            var tasks = new List<Task<MessageItem[]>>();

            foreach (var filename in filenames)
            {
                var tsk = LoadMessagesAsync(filename);
                tasks.Add(tsk);
            }

            await Task.WhenAll(tasks);

            var items = new List<MessageItem>();

            foreach (var tsk in tasks)
            {
                var item = tsk.Result;
                items.AddRange(item);
            }

            var ret = items.ToDictionary(t => t.Code, t => t.Value, StringComparer.OrdinalIgnoreCase);
            return ret;
        }

        private async Task<MessageItem[]> LoadMessagesAsync(string filename)
        {
            var currDir = AppContext.BaseDirectory;
            var path = Path.Combine(currDir, filename);

            var str = await File.ReadAllTextAsync(path);
            var ret = str.FromJson<MessageItem[]>();
            return ret!;
        }


        private class MessageItem
        {
            public string Code { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
        }
    }
}
