using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MOR.App
{
    public abstract class AbstractAppContext
    {
        private readonly IServiceScope SScope;
        protected IServiceProvider SP => SScope.ServiceProvider;


        public AbstractAppContext(IServiceProvider sp)
        {
            SScope = sp.CreateScope();
        }


        public IConfiguration Configuration => SP.GetRequiredService<IConfiguration>();

        public TService GetService<TService>() where TService : notnull => SP.GetRequiredService<TService>();

        public TService GetKeyedService<TService>(string key) where TService : notnull => SP.GetRequiredKeyedService<TService>(key);


        ~AbstractAppContext()
        {
            SScope.DisposeSafe();
        }
    }


    public abstract class AbstractWebAppContext : AbstractAppContext
    {
        public AbstractWebAppContext(IServiceProvider sp)
            : base(sp)
        {
        }
    }


    public abstract class AbstractServerlessAppContext : AbstractAppContext
    {
        public AbstractServerlessAppContext(IServiceProvider sp)
            : base(sp)
        {
        }
    }
}
