using System;
using System.Security.Cryptography.X509Certificates;

namespace BankId.Merchant.Library.AdvancedConfiguration
{
    internal class CertificateStoreLoader
    {
        public X509Certificate2 Load(string thumbprint)
        {
            return Get(thumbprint);
        }

        private X509Certificate2 Get(string thumbprint)
        {
            var cert = Get(thumbprint, StoreLocation.LocalMachine) ?? Get(thumbprint, StoreLocation.CurrentUser);
            if (cert == null)
            {
                throw new CommunicatorException("certificate not found");
            }
            return cert;
        }

        private X509Certificate2 Get(string thumbprint, StoreLocation storeLocation)
        {
            if (string.IsNullOrEmpty(thumbprint)) throw new ArgumentNullException(nameof(thumbprint));

            X509Store store = null;
            try
            {
                store = new X509Store(StoreName.My, storeLocation);
                store.Open(OpenFlags.ReadOnly | OpenFlags.IncludeArchived);
                X509Certificate2Collection col = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint,
                    false);

                if (col.Count != 0)
                {
                    return col[0];
                }
                return null;
            }
            finally
            {
                if (store != null)
                    store.Close();
            }
        }
    }
}
