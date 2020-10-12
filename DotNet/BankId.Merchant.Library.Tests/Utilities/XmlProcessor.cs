using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using BankId.Merchant.Library.Security;

namespace BankId.Server.Web.Models
{
    public class XmlProcessor
    {
        public static string XmlDsigRsaSha256Url = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
        public static X509Certificate2 ServerCertificate;
        public static X509Certificate2 AlternateServerCertificate;
        public static X509Certificate2 MerchantCertificate;
        public static string AssertionNamespaceUrl = "urn:oasis:names:tc:SAML:2.0:assertion";

        private const string ServerCertificateFileName = "BankID2020.QA.sha256.2048.cer";
        private const string AlternateServerCertificateName = "BankID2020.QA.sha256.2048.cer";

        public XmlProcessor(string certificateDir)
        {
            ServerCertificate = LoadCertificate(certificateDir, ServerCertificateFileName);
            AlternateServerCertificate = LoadCertificate(certificateDir, AlternateServerCertificateName);

            var merchantCertificateFile = new DirectoryInfo(certificateDir).EnumerateFiles("*.cer").First();
            Console.WriteLine("Merchant certificate file: " + merchantCertificateFile.Name);

            MerchantCertificate = new X509Certificate2(merchantCertificateFile.FullName, (string)null, X509KeyStorageFlags.MachineKeySet);

            CryptoConfig.AddAlgorithm(typeof(RSAPKCS1SHA256SignatureDescription), XmlDsigRsaSha256Url);
        }

        private static X509Certificate2 LoadCertificate(string certificateDir, string certificateFileName)
        {
            var certificateFile = new FileInfo(Path.Combine(certificateDir, certificateFileName));
            if (!certificateFile.Exists)
            {
                throw new Exception($"Please make sure the certificate {certificateFileName} exists in the folder {certificateDir}");
            }

            return new X509Certificate2(certificateFile.FullName, "", X509KeyStorageFlags.MachineKeySet);
        }

        public string AddSignature(string xml, bool useFingerprint = true, bool addReferenceURI = false)
        {
            return AddSignature(ServerCertificate, xml, useFingerprint, addReferenceURI);
        }

        private static string AddSignature(X509Certificate2 certificate, string xml, bool useFingerprint, bool addReferenceURI)
        {
            var doc = new XmlDocument
            {
                PreserveWhitespace = true
            };
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception)
            {
                return xml;
            }

            // document is empty, parse issue, etc
            if (doc.DocumentElement == null)
            {
                return xml;
            }

            // already has a signature on the root element
            if (doc.DocumentElement["Signature", SignedXml.XmlDsigNamespaceUrl] != null)
            {
                return xml;
            }

            var rsaKey = (RSACryptoServiceProvider)certificate.PrivateKey;

            var ki = new KeyInfo();
            if (useFingerprint)
            {
                ki.AddClause(new KeyInfoName(certificate.Thumbprint));
            }
            else
            {
                ki.AddClause(new KeyInfoX509Data(certificate));
            }

            var signedXml = new SignedXml(doc)
            {
                SigningKey = rsaKey
            };

            var reference = new Reference("")
            {
                DigestMethod = "http://www.w3.org/2001/04/xmlenc#sha256"
            };
            if (addReferenceURI)
            {
                reference.Uri = "#_a75adf55-01d7-40cc-929f-dbd8372ebdfc";
            }
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigExcC14NTransform());

            signedXml.Signature.SignedInfo.AddReference(reference);
            signedXml.Signature.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            signedXml.Signature.SignedInfo.SignatureMethod = XmlDsigRsaSha256Url;
            signedXml.Signature.KeyInfo = ki;

            signedXml.ComputeSignature();
            var signature = signedXml.GetXml().CloneNode(true);

            doc.DocumentElement.AppendChild(doc.ImportNode(signature, true));

            return doc.OuterXml;
        }
    }
}
