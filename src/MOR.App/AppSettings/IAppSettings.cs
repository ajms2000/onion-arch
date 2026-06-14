namespace MOR.App.AppSettings
{
    public interface IAppSettings
    {
        void Process(IServiceProvider? serviceProvider = null);
    }
}
