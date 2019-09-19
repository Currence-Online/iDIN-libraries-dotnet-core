using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using BankId.Merchant.Library.AdvancedConfiguration;
using BankId.Merchant.Library.AppConfig;

namespace BankId.Merchant.Library
{
    /// <summary>
    /// Configuration class
    /// </summary>
    public class Configuration : IConfiguration
    {
        private static IConfiguration _instance;

        /// <summary>
        /// Gets the active Configuration instance
        /// </summary>
        [Obsolete("Will be removed in future versions. Merchants are encouraged to create instances of the Configuration class instead.")]
        public static IConfiguration Instance
        {
            get { return _instance; }
            protected set { EnsureIsValid(value); _instance = value; }
        }

        /// <summary>
        /// Creates a configuration instance
        /// </summary>
        /// <param name="acquirerId">Acquirer ID</param>
        /// <param name="merchantId">ID of the Merchant</param>
        /// <param name="merchantReturnUrl">The merchant return URL to be used after successfull authentication</param>
        /// <param name="acquirerDirectoryUrl">The AcquirerDirectoryURL that the library will use to perform the DirectoryRequest</param>
        /// <param name="acquirerTransactionUrl">The AcquirerTransactionURL that the library will use to start a new authentication></param>
        /// <param name="acquirerStatusUrl">The AcquirerStatusURL that the library will use to get the status of an authentication</param>
        /// <param name="merchantCertificateFingerprint">The fingerprint of the merchant certificate</param>
        /// <param name="routingServiceCertificateFingerprint">The fingerprint of the Routing Service certificate</param>
        /// <param name="samlCertificateFingerprint">The fingerprint of the SAML certificate</param>
        /// <param name="serviceLogsLocation">Location of the service logs folder. The library will attempt to write service logs in the specified folder if service logs are enabled</param>
        /// <param name="serviceLogsEnabled">True if service logs are enabled, False otherwise</param>
        /// <param name="serviceLogsPattern">Pattern to be used when generating service logs files</param>
        public Configuration(string acquirerId, string merchantId, Uri merchantReturnUrl, Uri acquirerDirectoryUrl, Uri acquirerTransactionUrl, Uri acquirerStatusUrl, string merchantCertificateFingerprint, string routingServiceCertificateFingerprint, string samlCertificateFingerprint, string serviceLogsLocation, bool serviceLogsEnabled, string serviceLogsPattern)
            : this(acquirerId, merchantId, 0, merchantReturnUrl, acquirerDirectoryUrl, acquirerTransactionUrl, acquirerStatusUrl, merchantCertificateFingerprint, routingServiceCertificateFingerprint, null, samlCertificateFingerprint, serviceLogsLocation, serviceLogsEnabled, serviceLogsPattern)
        {
        }

        /// <summary>
        /// Creates a configuration instance
        /// </summary>
        /// <param name="acquirerId">Acquirer ID</param>
        /// <param name="merchantId">ID of the Merchant</param>
        /// <param name="merchantReturnUrl">The merchant return URL to be used after successfull authentication</param>
        /// <param name="acquirerDirectoryUrl">The AcquirerDirectoryURL that the library will use to perform the DirectoryRequest</param>
        /// <param name="acquirerTransactionUrl">The AcquirerTransactionURL that the library will use to start a new authentication></param>
        /// <param name="acquirerStatusUrl">The AcquirerStatusURL that the library will use to get the status of an authentication</param>
        /// <param name="merchantCertificate">The merchant certificate</param>
        /// <param name="routingServiceCertificate">The Routing Service certificate</param>
        /// <param name="samlCertificate">The fingerprint of the SAML certificate</param>
        /// <param name="serviceLogsLocation">Location of the service logs folder. The library will attempt to write service logs in the specified folder if service logs are enabled</param>
        /// <param name="serviceLogsEnabled">True if service logs are enabled, False otherwise</param>
        /// <param name="serviceLogsPattern">Pattern to be used when generating service logs files</param>
        public Configuration(string acquirerId, string merchantId, Uri merchantReturnUrl,
            Uri acquirerDirectoryUrl, Uri acquirerTransactionUrl, Uri acquirerStatusUrl,
            X509Certificate2 merchantCertificate, X509Certificate2 routingServiceCertificate,
            X509Certificate2 samlCertificate, string serviceLogsLocation, bool serviceLogsEnabled,
            string serviceLogsPattern)
            : this(acquirerId, merchantId, 0, merchantReturnUrl, acquirerDirectoryUrl, acquirerTransactionUrl,
                acquirerStatusUrl, merchantCertificate, routingServiceCertificate, samlCertificate, serviceLogsLocation, serviceLogsEnabled, serviceLogsPattern)
        {
        }

        /// <summary>
        /// Creates a configuration instance
        /// </summary>
        /// <param name="acquirerId">Acquirer ID</param>
        /// <param name="merchantId">ID of the Merchant</param>
        /// <param name="merchantSubId">The SubID that uniquely defines a trade name of the Merchant to be used for display</param>
        /// <param name="merchantReturnUrl">The merchant return URL to be used after successfull authentication</param>
        /// <param name="acquirerDirectoryUrl">The AcquirerDirectoryURL that the library will use to perform the DirectoryRequest</param>
        /// <param name="acquirerTransactionUrl">The AcquirerTransactionURL that the library will use to start a new authentication></param>
        /// <param name="acquirerStatusUrl">The AcquirerStatusURL that the library will use to get the status of an authentication</param>
        /// <param name="merchantCertificate">The merchant certificate</param>
        /// <param name="routingServiceCertificate">The Routing Service certificate</param>
        /// <param name="samlCertificate">The fingerprint of the SAML certificate</param>
        /// <param name="serviceLogsLocation">Location of the service logs folder. The library will attempt to write service logs in the specified folder if service logs are enabled</param>
        /// <param name="serviceLogsEnabled">True if service logs are enabled, False otherwise</param>
        /// <param name="serviceLogsPattern">Pattern to be used when generating service logs files</param>
        public Configuration(string acquirerId, string merchantId, uint merchantSubId, Uri merchantReturnUrl,
            Uri acquirerDirectoryUrl, Uri acquirerTransactionUrl, Uri acquirerStatusUrl,
            X509Certificate2 merchantCertificate, X509Certificate2 routingServiceCertificate,
            X509Certificate2 samlCertificate, string serviceLogsLocation, bool serviceLogsEnabled,
            string serviceLogsPattern)
            : this(acquirerId, merchantId, merchantSubId, merchantReturnUrl, acquirerDirectoryUrl, acquirerTransactionUrl, acquirerStatusUrl, serviceLogsLocation, serviceLogsEnabled, serviceLogsPattern)
        {
            MerchantCertificate = merchantCertificate;
            RoutingServiceCertificate = routingServiceCertificate;
            SamlCertificate = samlCertificate;

            MerchantCertificateFingerprint = MerchantCertificate.Thumbprint;
            RoutingServiceCertificateFingerprint = RoutingServiceCertificate.Thumbprint;
            SamlCertificateFingerprint = SamlCertificate.Thumbprint;
        }

        /// <summary>
        /// Creates a configuration instance
        /// </summary>
        /// <param name="acquirerId">Acquirer ID</param>
        /// <param name="merchantId">ID of the Merchant</param>
        /// <param name="merchantSubId">The SubID that uniquely defines a trade name of the Merchant to be used for display</param>
        /// <param name="merchantReturnUrl">The merchant return URL to be used after successfull authentication</param>
        /// <param name="acquirerDirectoryUrl">The AcquirerDirectoryURL that the library will use to perform the DirectoryRequest</param>
        /// <param name="acquirerTransactionUrl">The AcquirerTransactionURL that the library will use to start a new authentication></param>
        /// <param name="acquirerStatusUrl">The AcquirerStatusURL that the library will use to get the status of an authentication</param>
        /// <param name="merchantCertificate">The merchant certificate</param>
        /// <param name="routingServiceCertificate">The Routing Service certificate</param>
        /// <param name="alternateRoutingServiceCertificate">The Alternate Routing Service certificate</param>
        /// <param name="samlCertificate">The fingerprint of the SAML certificate</param>
        /// <param name="serviceLogsLocation">Location of the service logs folder. The library will attempt to write service logs in the specified folder if service logs are enabled</param>
        /// <param name="serviceLogsEnabled">True if service logs are enabled, False otherwise</param>
        /// <param name="serviceLogsPattern">Pattern to be used when generating service logs files</param>
        public Configuration(string acquirerId, string merchantId, uint merchantSubId, Uri merchantReturnUrl,
            Uri acquirerDirectoryUrl, Uri acquirerTransactionUrl, Uri acquirerStatusUrl,
            X509Certificate2 merchantCertificate, X509Certificate2 routingServiceCertificate, X509Certificate2 alternateRoutingServiceCertificate,
            X509Certificate2 samlCertificate, string serviceLogsLocation, bool serviceLogsEnabled,
            string serviceLogsPattern)
            : this(acquirerId, merchantId, merchantSubId, merchantReturnUrl, acquirerDirectoryUrl, acquirerTransactionUrl, acquirerStatusUrl, serviceLogsLocation, serviceLogsEnabled, serviceLogsPattern)
        {
            MerchantCertificate = merchantCertificate;
            RoutingServiceCertificate = routingServiceCertificate;
            AlternateRoutingServiceCertificate = alternateRoutingServiceCertificate;
            SamlCertificate = samlCertificate;

            MerchantCertificateFingerprint = MerchantCertificate.Thumbprint;
            RoutingServiceCertificateFingerprint = RoutingServiceCertificate.Thumbprint;
            AlternateRoutingServiceCertificateFingerprint = alternateRoutingServiceCertificate?.Thumbprint;
            SamlCertificateFingerprint = SamlCertificate.Thumbprint;
        }


        /// <summary>
        /// Creates a configuration instance
        /// </summary>
        /// <param name="acquirerId">Acquirer ID</param>
        /// <param name="merchantId">ID of the Merchant</param>
        /// <param name="merchantSubId">The SubID that uniquely defines a trade name of the Merchant to be used for display</param>
        /// <param name="merchantReturnUrl">The merchant return URL to be used after successfull authentication</param>
        /// <param name="acquirerDirectoryUrl">The AcquirerDirectoryURL that the library will use to perform the DirectoryRequest</param>
        /// <param name="acquirerTransactionUrl">The AcquirerTransactionURL that the library will use to start a new authentication></param>
        /// <param name="acquirerStatusUrl">The AcquirerStatusURL that the library will use to get the status of an authentication</param>
        /// <param name="merchantCertificateFingerprint">The fingerprint of the merchant certificate</param>
        /// <param name="routingServiceCertificateFingerprint">The fingerprint of the Routing Service certificate</param>
        /// <param name="alternateRoutingServiceCertificateFingerprint">The alternate fingerprint of the Routing Service certificate</param>
        /// <param name="samlCertificateFingerprint">The fingerprint of the SAML certificate</param>
        /// <param name="serviceLogsLocation">Location of the service logs folder. The library will attempt to write service logs in the specified folder if service logs are enabled</param>
        /// <param name="serviceLogsEnabled">True if service logs are enabled, False otherwise</param>
        /// <param name="serviceLogsPattern">Pattern to be used when generating service logs files</param>
        public Configuration(string acquirerId, string merchantId, uint merchantSubId, Uri merchantReturnUrl, Uri acquirerDirectoryUrl, Uri acquirerTransactionUrl, Uri acquirerStatusUrl, string merchantCertificateFingerprint, string routingServiceCertificateFingerprint, string alternateRoutingServiceCertificateFingerprint, string samlCertificateFingerprint, string serviceLogsLocation, bool serviceLogsEnabled, string serviceLogsPattern)
            : this(acquirerId, merchantId, merchantSubId, merchantReturnUrl, acquirerDirectoryUrl, acquirerTransactionUrl, acquirerStatusUrl, serviceLogsLocation, serviceLogsEnabled, serviceLogsPattern)
        {
            MerchantCertificateFingerprint = merchantCertificateFingerprint;
            RoutingServiceCertificateFingerprint = routingServiceCertificateFingerprint;
            AlternateRoutingServiceCertificateFingerprint = alternateRoutingServiceCertificateFingerprint;
            SamlCertificateFingerprint = samlCertificateFingerprint;

            var certLoader = new CertificateStoreLoader();
            MerchantCertificate = certLoader.Load(MerchantCertificateFingerprint);
            RoutingServiceCertificate = certLoader.Load(RoutingServiceCertificateFingerprint);
            if (!string.IsNullOrEmpty(AlternateRoutingServiceCertificateFingerprint))
            {
                AlternateRoutingServiceCertificate = certLoader.Load(AlternateRoutingServiceCertificateFingerprint);
            }
            SamlCertificate = certLoader.Load(SamlCertificateFingerprint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="acquirerId">Acquirer ID</param>
        /// <param name="merchantId">ID of the Merchant</param>
        /// <param name="merchantSubId">The SubID that uniquely defines a trade name of the Merchant to be used for display</param>
        /// <param name="merchantReturnUrl">The merchant return URL to be used after successfull authentication</param>
        /// <param name="acquirerDirectoryUrl">The AcquirerDirectoryURL that the library will use to perform the DirectoryRequest</param>
        /// <param name="acquirerTransactionUrl">The AcquirerTransactionURL that the library will use to start a new authentication></param>
        /// <param name="acquirerStatusUrl">The AcquirerStatusURL that the library will use to get the status of an authentication</param>
        /// <param name="serviceLogsLocation">Location of the service logs folder. The library will attempt to write service logs in the specified folder if service logs are enabled</param>
        /// <param name="serviceLogsEnabled">True if service logs are enabled, False otherwise</param>
        /// <param name="serviceLogsPattern">Pattern to be used when generating service logs files</param>
        private Configuration(string acquirerId, string merchantId, uint merchantSubId, Uri merchantReturnUrl, Uri acquirerDirectoryUrl, Uri acquirerTransactionUrl, Uri acquirerStatusUrl, string serviceLogsLocation, bool serviceLogsEnabled, string serviceLogsPattern)
        {
            AcquirerId = acquirerId;
            MerchantId = merchantId;
            MerchantSubId = merchantSubId;
            MerchantReturnUrl = merchantReturnUrl;
            AcquirerDirectoryUrl = acquirerDirectoryUrl;
            AcquirerTransactionUrl = acquirerTransactionUrl;
            AcquirerStatusUrl = acquirerStatusUrl;

            ServiceLogsLocation = serviceLogsLocation;
            ServiceLogsEnabled = serviceLogsEnabled;
            ServiceLogsPattern = string.IsNullOrWhiteSpace(serviceLogsPattern) ? @"%Y-%M-%D\%h%m%s.%f-%a.xml" : serviceLogsPattern;
        }

        /// <summary>
        /// Sets the Configuration object to be used by Communicator instances
        /// </summary>
        /// <param name="configuration">The object implementing IConfiguration to use</param>
        public static void Setup(IConfiguration configuration)
        {
            try
            {
                Instance = configuration;
            }
            catch (Exception e)
            {
                throw new CommunicatorException("Cannot load configuration: " + e.Message, e);
            }
        }
      

        /// <summary>
        /// Attempts to load the settings from the application's configuration
        /// </summary>
        public static void Load()
        {
            try
            {
                var config = AppConfiguration.AppSettings();

                bool serviceLogsEnabled;
                if (!bool.TryParse(config["BankId.ServiceLogs.Enabled"], out serviceLogsEnabled))
                    serviceLogsEnabled = true;

                var merchantSubIdString = config["BankId.Merchant.SubID"];
                uint merchantSubId = 0;
                if (!string.IsNullOrWhiteSpace(merchantSubIdString))
                {
                    uint.TryParse(merchantSubIdString, out merchantSubId);
                }

                Instance = new Configuration(
                    config["BankId.Merchant.AcquirerID"],
                    config["BankId.Merchant.MerchantID"],
                    merchantSubId,
                    ParseUri("BankId.Merchant.ReturnUrl"),
                    ParseUri("BankId.Acquirer.DirectoryUrl"),
                    ParseUri("BankId.Acquirer.TransactionUrl"),
                    ParseUri("BankId.Acquirer.StatusUrl"),
                    config["BankId.Merchant.Certificate.Fingerprint"],
                    config["BankId.RoutingService.Certificate.Fingerprint"],
                    config["BankId.RoutingService.AlternateCertificate.Fingerprint"],
                    config["BankId.SAML.Certificate.Fingerprint"],
                    config["BankId.ServiceLogs.Location"],
                    serviceLogsEnabled,
                    config["BankId.ServiceLogs.Pattern"]
                );

            }
            catch (Exception e)
            {
                throw new CommunicatorException("Cannot load configuration: " + e.Message, e);
            }
        }

        private static Uri ParseUri(string appSettingsKey)
        {
            var str = AppConfiguration.AppSettings()[appSettingsKey];
            return str.ParseAsUri(new ArgumentException("The configuration parameter is not configured.", appSettingsKey));
        }

        /// <summary>
        /// Ensures that the configuration is valid.
        /// </summary>
        /// <param name="config"></param>
        protected static void EnsureIsValid(IConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            CheckNullOrWhitespace(config.AcquirerId, "BankId.Acquirer.AcquirerID");
            CheckNullOrWhitespace(config.MerchantId, "BankId.Merchant.MerchantID");

            CheckNull(config.RoutingServiceCertificate, "BankId.RoutingService.Certificate");
            CheckNull(config.MerchantCertificate, "BankId.Merchant.Certificate");
            CheckNull(config.SamlCertificate, "BankId.SAML.Certificate");

            CheckNull(config.MerchantReturnUrl, "BankId.Merchant.ReturnUrl");
            CheckNull(config.AcquirerDirectoryUrl, "BankId.Acquirer.DirectoryUrl");
            CheckNull(config.AcquirerTransactionUrl, "BankId.Acquirer.TransactionUrl");
            CheckNull(config.AcquirerStatusUrl, "BankId.Acquirer.StatusUrl");

        }

        private static void CheckNullOrWhitespace(string setting, string settingName)
        {
            if (string.IsNullOrWhiteSpace(setting))
                throw new ArgumentException("The configuration parameter is not configured.", settingName);
        }

        private static void CheckNull(object setting, string settingName)
        {
            if (setting == null)
                throw new ArgumentException("The configuration parameter is not configured.", settingName);
        }


        /// <summary>
        /// Acquirer ID
        /// </summary>
        public string AcquirerId { get; set; }

        /// <summary>
        /// Merchant Id
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// BankID contract registration number sub of the Merchant. The SubID that uniquely defines a trade name of the Merchant to be used for display.
        /// </summary>
        public uint MerchantSubId { get; set; }

        /// <summary>
        /// A valid URL to which the Consumer is redirected to after authentication
        /// </summary>
        public Uri MerchantReturnUrl { get; set; }

        /// <summary>
        /// The URL to which the library sends GetDirectory request messages
        /// </summary>
        public Uri AcquirerDirectoryUrl { get; set; }

        /// <summary>
        /// The URL to which the library sends authentication request messages
        /// </summary>
        public Uri AcquirerTransactionUrl { get; set; }

        /// <summary>
        /// The URL to which the library sends Status request messages
        /// </summary>
        public Uri AcquirerStatusUrl { get; set; }

        /// <summary>
        /// A directory on the disk where the library saves ISO pain raw messages.
        /// </summary>
        public string ServiceLogsLocation { get; set; }

        /// <summary>
        /// This tells the library that it should save ISO pain raw messages or not. Default is true.
        /// </summary>
        public bool ServiceLogsEnabled { get; set; }

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
        public string ServiceLogsPattern { get; set; }

        /// <summary>
        /// A string which specifies the fingerprint of the certificate to use to sign messages to the Routing Service.
        /// </summary>
        public string MerchantCertificateFingerprint { get; set; }

        /// <summary>
        /// A string which specifies the fingerprint of the certificate to use to validate messages from the Routing Service.
        /// </summary>
        public string RoutingServiceCertificateFingerprint { get; set; }

        /// <summary>
        /// A string which specifies the alternate fingerprint of the certificate to use to validate messages from the Routing Service.
        /// </summary>
        public string AlternateRoutingServiceCertificateFingerprint { get; set; }

        /// <summary>
        /// A string which specifies the fingerprint of the certificate necesary to decrypt the SAML request - may be identical to the Merchant Certificate
        /// </summary>
        public string SamlCertificateFingerprint { get; set; }

        /// <summary>
        /// You may overwrite the signing certificate (which was loaded using MerchantCertificateFingerprint), if you want
        /// to load it using a different method.
        /// </summary>
        public X509Certificate2 MerchantCertificate { get; set; }

        /// <summary>
        /// You may overwrite the acquirer certificate (which was loaded using RoutingServiceCertificateFingerprint), if you want
        /// to load it using a different method.
        /// </summary>
        public X509Certificate2 RoutingServiceCertificate { get; set; }
        
        /// <summary>
        /// You may overwrite the acquirer certificate (which was loaded using AlternateRoutingServiceCertificateFingerprint), if you want
        /// to load it using a different method.
        /// </summary>
        public X509Certificate2 AlternateRoutingServiceCertificate { get; set; }

        /// <summary>
        ///  You may overwrite the SAML certificate (which was loaded using SamlCertificateFingerprint), if you want
        /// to load it using a different method.
        /// </summary>
        public X509Certificate2 SamlCertificate { get; set; }

        /// <summary>
        /// Gets the default Logger instance
        /// </summary>
        /// <returns></returns>
        public virtual ILogger GetLogger()
        {
            return new Logger(this);
        }

        /// <summary>
        /// Gets the default Messenger instance
        /// </summary>
        /// <returns></returns>
        public virtual IMessenger GetMessenger()
        {
            return new Messenger(this);
        }
    }


    internal static class ConversionExtensions
    {
        internal static Uri ParseAsUri(this string str, Exception parseException)
        {
            Uri uri;
            if (!Uri.TryCreate(str, UriKind.Absolute, out uri))
                throw parseException;

            return uri;
        }
    }
}
