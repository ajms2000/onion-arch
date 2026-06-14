using Microsoft.Extensions.Configuration;

namespace System.Configuration
{
    internal class CombinedEnvironmentVariablesConfigurationProvider : ConfigurationProvider
    {
        private const string MySqlServerPrefix = "MYSQLCONNSTR_";
        private const string SqlAzureServerPrefix = "SQLAZURECONNSTR_";
        private const string SqlServerPrefix = "SQLCONNSTR_";
        private const string CustomConnectionStringPrefix = "CUSTOMCONNSTR_";

        private readonly string _prefix;
        private readonly string _normalizedPrefix;


        public CombinedEnvironmentVariablesConfigurationProvider()
        {
            _prefix = string.Empty;
            _normalizedPrefix = string.Empty;
        }

        public CombinedEnvironmentVariablesConfigurationProvider(string prefix)
        {
            _prefix = prefix ?? string.Empty;
            _normalizedPrefix = Normalize(_prefix);
        }


        public override void Load()
        {
            var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

            LoadEnvVariables(EnvironmentVariableTarget.Machine, data);
            LoadEnvVariables(EnvironmentVariableTarget.User, data);
            LoadEnvVariables(EnvironmentVariableTarget.Process, data);

            Load(data);
        }

        public override string ToString()
        {
            string s = GetType().Name;
            if (!string.IsNullOrEmpty(_prefix))
            {
                s += $" Prefix: '{_prefix}'";
            }
            return s;
        }


        internal void Load(Dictionary<string, string?> envVariables)
        {
            var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

            foreach (var key in envVariables.Keys)
            {
                var value = envVariables[key];

                if (key.StartsWith(MySqlServerPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    HandleMatchedConnectionStringPrefix(data, MySqlServerPrefix, "MySql.Data.MySqlClient", key, value);
                }
                else if (key.StartsWith(SqlAzureServerPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    HandleMatchedConnectionStringPrefix(data, SqlAzureServerPrefix, "System.Data.SqlClient", key, value);
                }
                else if (key.StartsWith(SqlServerPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    HandleMatchedConnectionStringPrefix(data, SqlServerPrefix, "System.Data.SqlClient", key, value);
                }
                else if (key.StartsWith(CustomConnectionStringPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    HandleMatchedConnectionStringPrefix(data, CustomConnectionStringPrefix, null, key, value);
                }
                else
                {
                    AddIfNormalizedKeyMatchesPrefix(data, Normalize(key), value);
                }
            }

            Data = data;
        }


        private void HandleMatchedConnectionStringPrefix(Dictionary<string, string?> data, string connectionStringPrefix, string? provider, string fullKey, string? value)
        {
            string normalizedKeyWithoutConnectionStringPrefix = Normalize(fullKey.Substring(connectionStringPrefix.Length));

            // Add the key-value pair for connection string, and optionally provider name
            AddIfNormalizedKeyMatchesPrefix(data, $"ConnectionStrings:{normalizedKeyWithoutConnectionStringPrefix}", value);
            if (provider != null)
            {
                AddIfNormalizedKeyMatchesPrefix(data, $"ConnectionStrings:{normalizedKeyWithoutConnectionStringPrefix}_ProviderName", provider);
            }
        }

        private void AddIfNormalizedKeyMatchesPrefix(Dictionary<string, string?> data, string normalizedKey, string? value)
        {
            if (normalizedKey.StartsWith(_normalizedPrefix, StringComparison.OrdinalIgnoreCase))
            {
                data[normalizedKey.Substring(_normalizedPrefix.Length)] = value;
            }
        }

        private static void LoadEnvVariables(EnvironmentVariableTarget from, Dictionary<string, string?> target)
        {
            var source = Environment.GetEnvironmentVariables(from);

            var e = source.GetEnumerator();

            try
            {
                while (e.MoveNext())
                {
                    var key = (string)e.Entry.Key;
                    var value = (string?)e.Entry.Value;

                    if (!target.ContainsKey(key))
                    {
                        target.TryAdd(key, value);
                    }
                    else
                    {
                        target[key] = value;
                    }
                }
            }
            finally
            {
                (source as IDisposable)?.Dispose();
            }
        }

        private static string Normalize(string key) => key.Replace("__", ConfigurationPath.KeyDelimiter);
    }
}
