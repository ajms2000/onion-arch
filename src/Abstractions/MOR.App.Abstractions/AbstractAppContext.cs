using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MOR.App
{
    /// <summary>
    /// IMPORTANT: Do not register this as Singleton in DI container.
    /// </summary>
    public abstract class AbstractAppContext
    {
        protected readonly IServiceProvider SP;


        public AbstractAppContext(IServiceProvider sp)
        {
            SP = sp;
        }


        public IConfiguration Configuration => SP.GetRequiredService<IConfiguration>();

        public TService GetService<TService>() where TService : notnull => SP.GetRequiredService<TService>();

        public TService GetKeyedService<TService>(string key) where TService : notnull => SP.GetRequiredKeyedService<TService>(key);
    }


    /// <summary>
    /// IMPORTANT: Do not register this as Singleton in DI container.
    /// </summary>
    public abstract class AbstractWebAppContext : AbstractAppContext
    {
        public AbstractWebAppContext(IServiceProvider sp)
            : base(sp)
        {
        }
    }


    /// <summary>
    /// IMPORTANT: Do not register this as Singleton in DI container.
    /// </summary>
    public abstract class AbstractServerlessAppContext : AbstractAppContext
    {
        public AbstractServerlessAppContext(IServiceProvider sp)
            : base(sp)
        {
        }
    }
}
