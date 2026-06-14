namespace MOR.App.AppSettings
{
    public abstract class BaseAppSettings : IAppSettings
    {
        public virtual void Process(IServiceProvider? serviceProvider = null)
        {
            // Do nothing. Derived class extend as necessary.
        }
    }
}
