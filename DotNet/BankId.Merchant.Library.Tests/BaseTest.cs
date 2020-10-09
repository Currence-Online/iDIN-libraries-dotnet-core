using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using BankId.Merchant.Library.Security;
using BankId.Merchant.Library.Tests.Utilities;
using BankId.Merchant.Library.Tests.Utilities;
using Moq;

namespace BankId.Merchant.Library.Tests
{
    public class BaseTest
    {
        protected readonly Regex dateTimeRegex = new Regex(@"[\d]{4}-[\d]{2}-[\d]{2}T[\d]{2}:[\d]{2}:[\d]{2}.[\d]{3}Z");
        protected TestMessenger messenger = new TestMessenger(TestMessage.Get("StatusResponse-Sample"));
        protected Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
        protected Mock<ILogger> mockLogger = new Mock<ILogger>();
        protected Uri merchantReturnUrl = new Uri("http://localhost");

        protected void SetupConfiguration()
        {
            var libraryCert = new X509Certificate2(Path.Combine(Directories.Certificates, "BankID2020.Libs.sha256.2048.csp.p12"), "123456");
            var serverCert = new X509Certificate2(Path.Combine(Directories.Certificates, "BankID2020.QA.sha256.2048.cer"));


            mockConfig.Setup(x => x.MerchantId).Returns("1234567890");
            mockConfig.Setup(x => x.AcquirerId).Returns("42");
            mockConfig.Setup(x => x.GetMessenger()).Returns(messenger);
            mockConfig.Setup(x => x.GetLogger()).Returns(mockLogger.Object);

            mockConfig.Setup(x => x.RoutingServiceCertificateFingerprint).Returns(serverCert.Thumbprint);
            mockConfig.Setup(x => x.RoutingServiceCertificate).Returns(serverCert);
            //mockConfig.Setup(x => x.GetXmlSecurity()).Returns(new XmlSecurity(mockConfig.Object));

            mockConfig.Setup(x => x.MerchantCertificateFingerprint).Returns(libraryCert.Thumbprint);
            mockConfig.Setup(x => x.MerchantCertificate).Returns(libraryCert);
            mockConfig.Setup(x => x.SamlCertificateFingerprint).Returns(libraryCert.Thumbprint);
            mockConfig.Setup(x => x.SamlCertificate).Returns(libraryCert);

            mockConfig.Setup(x => x.MerchantReturnUrl).Returns(merchantReturnUrl);

            mockConfig.Setup(x => x.AcquirerDirectoryUrl).Returns(new Uri("http://example.com/directory"));
            mockConfig.Setup(x => x.AcquirerStatusUrl).Returns(new Uri("http://example.com/status"));
            mockConfig.Setup(x => x.AcquirerTransactionUrl).Returns(new Uri("http://example.com/transaction"));

        }
    }
}