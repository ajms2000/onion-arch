using System.Security.Cryptography.X509Certificates;

namespace System
{
    public static class CertificateUtilities
    {
        // For Azure
        // https://docs.microsoft.com/en-us/azure/app-service/configure-ssl-certificate-in-code#load-certificate-in-windows-apps
        // https://docs.microsoft.com/en-us/azure/app-service/environment/certificates#private-client-certificate

        public static X509Certificate2 LoadStoreCertificate(string thumbprint, StoreName storeName, StoreLocation storeLocation)
        {
            using (var certStore = new X509Store(storeName, storeLocation))
            {
                certStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                var certCollection = certStore.Certificates.Find(
                    X509FindType.FindByThumbprint,
                    thumbprint,
                    false);

                if (certCollection.Count > 0)
                {
                    var cert = certCollection[0];
                    return cert;
                }
            }

            throw new AppException("Requested certificate does not exist. " + $"{thumbprint}-{storeName}-{storeLocation}");
        }

        // Fix for 'Invalid provider type specified'
        // https://community.xero.com/developer/discussion/67424681
        public static X509Certificate2 GetCertificate(byte[] data, string? password = null)
        {
            var ret = default(X509Certificate2);

            if (password.NotNullOrWhiteSpace())
            {
                ret = X509CertificateLoader.LoadPkcs12(data, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            }
            else
            {
                ret = X509CertificateLoader.LoadCertificate(data);
            }

            return ret;
        }
    }
}
