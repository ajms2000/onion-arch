namespace System.Data
{
    public class DBConnectionStringProperties
    {
        public string? DataSource { get; set; }
        public string? InitialCatalog { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool IntegratedSecuity { get; set; }
        public bool MultipleActiveResultSets { get; set; }
        public bool EnableColumnEncryption { get; set; }
        public string? App { get; set; }
    }
}
