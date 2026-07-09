namespace MOR.Services
{
    public interface ICodeMessageProvider
    {
        ValueTask<string?> GetMessageAsync(string code, string? defaultValue = null);
        string? GetMessage(string code, string? defaultValue = null);
    }
}
