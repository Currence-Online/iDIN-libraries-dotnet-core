using System.Security.Cryptography.X509Certificates;

namespace BankId.Merchant.Library.AdvancedConfiguration
{
    public interface ICertificateLoader
    {
        X509Certificate2 Load(string thumbprint);
    }
}