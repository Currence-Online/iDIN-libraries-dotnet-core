using System;
using System.Security.Cryptography.X509Certificates;

namespace BankId.Merchant.Library
{
    /// <summary>
    /// Interface that describes the configuration settings for the library, which are tied with each ICommunicator instance:
    /// when you instantiate a Communicator object, it attempts to load its configuration using App.config or Web.config.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Acquirer ID
        /// </summary>
        string AcquirerId { get; set; }

        /// <summary>
        /// Merchant Id
        /// </summary>
        string MerchantId { get; set; }

        /// <summary>
        /// BankID contract registration number sub of the Merchant. The SubID that uniquely defines a trade name of the Merchant to be used for display.
        /// </summary>
        uint MerchantSubId { get; set; }

        /// <summary>
        /// A valid URL to which the Consumer is redirected after successfull authentication.
        /// </summary>
        Uri MerchantReturnUrl { get; set; }

        /// <summary>
        /// A string which specifies the fingerprint of the certificate to use to sign messages to the Routing Service.
        /// </summary>
        string MerchantCertificateFingerprint { get; set; }

        /// <summary>
        /// A string which specifies the fingerprint of the certificate to use to validate messages from the Routing Service.
        /// </summary>
        string RoutingServiceCertificateFingerprint { get; set; }
       
        /// <summary>
        /// A string which specifies the fingerprint of the alternate certificate to use to validate messages from the Routing Service.
        /// </summary>
        string AlternateRoutingServiceCertificateFingerprint { get; set; }

        /// <summary>
        /// A string which specifies the fingerprint of the certificate necesary to decrypt the SAML request - may be identical to the Merchant Certificate
        /// </summary>
        string SamlCertificateFingerprint { get; set; }

        /// <summary>
        /// You may overwrite the SAML certificate (which was loaded using SamlCertificateFingerprint), if you want
        /// to load it using a different method.
        /// </summary>
        X509Certificate2 SamlCertificate { get; set; }

        /// <summary>
        /// You may overwrite the signing certificate (which was loaded using MerchantCertificateFingerprint), if you want
        /// to load it using a different method.
        /// </summary>
        X509Certificate2 MerchantCertificate { get; set; }

        /// <summary>
        /// You may overwrite the acquirer certificate (which was loaded using RoutingServiceCertificateFingerprint), if you want
        /// to load it using a different method.
        /// </summary>
        X509Certificate2 RoutingServiceCertificate { get; set; }
        /// <summary>
        /// You may overwrite the acquirer certificate (which was loaded using AlternateRoutingServiceCertificateFingerprint), if you want
        /// to load it using a different method.
        /// </summary>
        X509Certificate2 AlternateRoutingServiceCertificate { get; set; }

        /// <summary>
        /// The URL to which the library sends GetDirectory request messages
        /// </summary>
        Uri AcquirerDirectoryUrl { get; set; }

        /// <summary>
        /// The URL to which the library sends authentication request messages
        /// </summary>
        Uri AcquirerTransactionUrl { get; set; }

        /// <summary>
        /// The URL to which the library sends Status request messages
        /// </summary>
        Uri AcquirerStatusUrl { get; set; }

        /// <summary>
        /// This tells the library that it should save ISO pain raw messages or not. Default is true.
        /// </summary>
        bool ServiceLogsEnabled { get; set; }

        /// <summary>
        /// A directory on the disk where the library saves ISO pain raw messages.
        /// </summary>
        string ServiceLogsLocation { get; set; }

        /// <summary>
        /// A string that describes a pattern to distinguish the ISO pain raw messages. For example,
        /// %Y-%M-%D\%h%m%s.%f-%a.xml -> 102045.924-AcquirerTrxReq.xml
        /// </summary>
        /// <remarks>
        /// %Y = current year
        /// %M = current month
        /// %D = current day
        /// %h = current hour
        /// %m = current minute
        /// %s = current second
        /// %f = current millisecond
        /// %a = current action
        /// </remarks>
        string ServiceLogsPattern { get; set; }

        /// <summary>
        /// Gets the default Logger instance
        /// </summary>
        /// <returns></returns>
        ILogger GetLogger();

        /// <summary>
        /// Gets the default Messenger instance
        /// </summary>
        /// <returns></returns>
        IMessenger GetMessenger();
    }
}
