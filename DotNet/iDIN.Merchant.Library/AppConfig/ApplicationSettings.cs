using System;

namespace BankId.Merchant.Library.AppConfig
{
    public class ApplicationSettings
    {
        public string MerchantAcquirerId { get; set; }
        public string MerchantMerchantId { get; set; }
        public uint MerchantSubId { get; set; }
        public Uri MerchantReturnUrl { get; set; }
        public Uri AcquirerDirectoryUrl { get; set; }
        public Uri AcquirerTransactionUrl { get; set; }
        public Uri AcquirerStatusUrl { get; set; }
        public string MerchantCertificateFingerprint { get; set; }
        public string RoutingServiceCertificateFingerprint { get; set; }
        public string AlternateRoutingServiceCertificateFingerprint { get; set; }
        public string SamlCertificateFingerprint { get; set; }
        public string ServiceLogsLocation { get; set; }
        public bool ServiceLogsEnabled { get; set; }
        public string ServiceLogsPattern { get; set; }
    }
}
