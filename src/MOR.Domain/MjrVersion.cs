namespace MOR
{
    public class MjrVersion
    {
        public const string DefaultVersion = "1.0.0";

        private static Type AsmTypeInfVersion = typeof(System.Reflection.AssemblyInformationalVersionAttribute);
        private static MjrVersion? _MjrVersion;

        public string Version { get; set; } = DefaultVersion;
        public string ProductVersion { get; set; } = DefaultVersion;


        public static MjrVersion GetMjrVersion()
        {
            if (_MjrVersion == null)
            {
                var asm = System.Reflection.Assembly.GetExecutingAssembly();
                var asmName = asm.GetName();
                var asmVersion = ConvertToSemVer(asmName.Version);

                var attrInfVer = asm.GetCustomAttributes(AsmTypeInfVersion, false).FirstOrDefault() as System.Reflection.AssemblyInformationalVersionAttribute;

                _MjrVersion = new MjrVersion
                {
                    Version = asmVersion,
                    ProductVersion = attrInfVer?.InformationalVersion ?? MjrVersion.DefaultVersion,
                };
            }

            return _MjrVersion;
        }

        private static string ConvertToSemVer(Version? version)
        {
            if (version != null)
            {
                var asmVersion = $"{version.Major}.{version.Minor}.{version.Build}";
                return asmVersion;
            }

            return DefaultVersion;
        }
    }
}
