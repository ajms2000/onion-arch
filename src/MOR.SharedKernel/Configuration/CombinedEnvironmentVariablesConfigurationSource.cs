using Microsoft.Extensions.Configuration;

namespace System.Configuration
{
    public class CombinedEnvironmentVariablesConfigurationSource : IConfigurationSource
    {
        public string Prefix { get; set; } = string.Empty;


        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new CombinedEnvironmentVariablesConfigurationProvider(Prefix);
        }
    }
}
