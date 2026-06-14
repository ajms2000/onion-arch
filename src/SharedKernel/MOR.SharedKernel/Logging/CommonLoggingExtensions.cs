using Microsoft.Extensions.DependencyInjection;

namespace System.Logging
{
    public static class CommonLoggingExtensions
    {
        public static void RegisterILogger<TMarker>(this IServiceCollection services)
        {
            // By default framework doesn't provide ILogger but only ILogger<TCategory>. So below code will circumvent it.
            services.AddTransient<Microsoft.Extensions.Logging.ILogger>(s => s.GetRequiredService<Microsoft.Extensions.Logging.ILogger<TMarker>>());
        }
    }
}
