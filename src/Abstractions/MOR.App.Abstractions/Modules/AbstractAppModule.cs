using MOR.Services;

namespace MOR.App.Modules
{
    public abstract class AbstractAppModule<TAppContext>
        where TAppContext : AbstractAppContext
    {
        protected readonly TAppContext Context;


        public AbstractAppModule(TAppContext context)
        {
            Context = context;
        }


        protected string GetCodeMessage(string code, string? defaultValue = null)
        {
            var msgProv = Context.GetService<ICodeMessageProvider>();

            var ret = msgProv.GetMessage(code, defaultValue);
            return ret;
        }
    }


    public abstract class AbstractApiAppModule<TAppContext> : AbstractAppModule<TAppContext>
        where TAppContext : AbstractWebAppContext
    {
        public AbstractApiAppModule(TAppContext context)
            : base(context)
        {
        }
    }

    public abstract class AbstractServerlessAppModule<TAppContext> : AbstractAppModule<TAppContext>
        where TAppContext : AbstractServerlessAppContext
    {
        public AbstractServerlessAppModule(TAppContext context)
            : base(context)
        {
        }
    }
}
