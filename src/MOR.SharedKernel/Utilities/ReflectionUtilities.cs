using System.Text;

namespace System
{
    public static class ReflectionUtilities
    {
        public static Stream? GetEmbeddedDataStreamFromResource(string resourceName)
        {
            var contentStream = default(Stream);
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            var formattedResName = resourceName.Replace('\\', '.');
            formattedResName = formattedResName.Replace('/', '.');

            foreach (var asm in asms)
            {
                if (!asm.IsDynamic)
                {
                    var resNames = asm.GetManifestResourceNames();
                    var name = resNames.SingleOrDefault(t => t.EndsWith(formattedResName, StringComparison.InvariantCultureIgnoreCase));

                    if (name != null)
                    {
                        contentStream = asm.GetManifestResourceStream(name);
                        break;
                    }
                }
            }

            return contentStream;
        }

        public static string? GetEmbeddedDataFromResourceAsString(string resourceName, Encoding? encoding = null)
        {
            var ret = default(string);
            var contentStream = GetEmbeddedDataStreamFromResource(resourceName);

            if (contentStream != null)
            {
                var sr = encoding == null ? new StreamReader(contentStream) : new StreamReader(contentStream, encoding);

                using (sr)
                {
                    ret = sr.ReadToEnd();
                }
            }

            return ret;
        }

        /// <param name="encoding">Default: UTF8</param>
        public static byte[]? GetEmbeddedDataFromResourceAsBytes(string resourceName, Encoding? encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            var ret = default(byte[]);
            var contentStream = GetEmbeddedDataStreamFromResource(resourceName);

            if (contentStream != null)
            {
                using (var br = new BinaryReader(contentStream))
                {
                    ret = br.ReadAllBytes();
                }
            }

            return ret;
        }
    }
}
