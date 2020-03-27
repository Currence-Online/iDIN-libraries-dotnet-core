using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using BankId.Merchant.Library.Xml.Schemas.iDx;
using BankId.Merchant.Library.Xml.Schemas.Saml.AuthContext;
using BankId.Merchant.Library.Xml.Schemas.Saml.Metadata;
using BankId.Merchant.Library.Xml.Schemas.Saml.Protocol;
using BankId.Merchant.Library.Xml.Schemas.Saml.Xml;
using SignatureType = BankId.Merchant.Library.Xml.Schemas.iDx.SignatureType;

namespace BankId.Merchant.Library.Security
{
    internal class XmlSecurity : IXmlSecurity
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        private static readonly XmlSchemaSet SchemaSet;
        private static readonly Dictionary<Type, string> TypeSchemaMapping = new Dictionary<Type, string>
        {
            { typeof(SignatureType), "xmldsig-core-schema.xsd" },

            { typeof(DirectoryReq), "idx.merchant-acquirer.1.0.xsd" },
            { typeof(DirectoryRes), "idx.merchant-acquirer.1.0.xsd" },
            { typeof(AcquirerTrxReq), "idx.merchant-acquirer.1.0.xsd" },
            { typeof(AcquirerTrxRes), "idx.merchant-acquirer.1.0.xsd" },
            { typeof(AcquirerStatusReq), "idx.merchant-acquirer.1.0.xsd" },
            { typeof(AcquirerStatusRes), "idx.merchant-acquirer.1.0.xsd" },
            { typeof(AcquirerErrorRes), "idx.merchant-acquirer.1.0.xsd" },

            { typeof(AssertionType), "Schemas.saml-schema-assertion-2.0.xsd" },
            { typeof(CipherDataType), "Schemas.xenc-schema.xsd" },
            { typeof(AuthenticatorTransportProtocolType), "Schemas.saml-schema-authn-context-2.0.xsd" },
            { typeof(DumymyXmlClass), "Schemas.xml.xsd" },
            { typeof(AdditionalMetadataLocationType), "Schemas.saml-schema-metadata-2.0.xsd" },
            { typeof(AuthnRequestType), "Schemas.saml-schema-protocol-2.0.xsd" }

        };

        static XmlSecurity()
        {
            SchemaSet = new XmlSchemaSet();

            var currentType = typeof(XmlSecurity);
            Debug.Assert(currentType.Namespace != null, "currentType.Namespace != null");
            var typeNamespace = currentType.Namespace.Substring(0, currentType.Namespace.LastIndexOf("Security", StringComparison.Ordinal)) + "Xml";

            foreach (var pair in TypeSchemaMapping)
            {
                var xmlRoot = (XmlTypeAttribute)Attribute.GetCustomAttribute(pair.Key, typeof(XmlTypeAttribute));

                if (!SchemaSet.Contains(xmlRoot.Namespace))
                {
                    using (var stream = currentType.Assembly.GetManifestResourceStream(typeNamespace + "." + pair.Value))
                    {
                        if (stream == null)
                            throw new Exception(string.Format("Error on getting resource {0}.{1}", typeNamespace, pair.Value));

                        stream.Position = 0;
                        SchemaSet.Add(xmlRoot.Namespace, XmlReader.Create(stream));
                    }
                }
            }

            SchemaSet.Compile();

            // this prevents an exception: SignatureDescription could not be created for the signature algorithm supplied.
            // Apparently System.Security.Cryptography.Xml.SignedXml can't deal with SHA256 by default
            CryptoConfig.AddAlgorithm(typeof(RSAPKCS1SHA256SignatureDescription), "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");
        }

        public XmlSecurity(IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            _configuration = configuration;
            _logger = configuration.GetLogger();
        }

        public string AddSignature(string xml)
        {
            _logger.Log("signing xml...");
            var xmlDoc = new XmlDocument { PreserveWhitespace = true };
            xmlDoc.LoadXml(xml);

            var certificate = _configuration.MerchantCertificate;

            XmlSignature.Sign(ref xmlDoc, certificate, xmlDoc.DocumentElement);

            var stringWriter = new StringWriter();
            var xmlTextWriter = XmlWriter.Create(stringWriter);

            xmlDoc.WriteTo(xmlTextWriter);
            xmlTextWriter.Flush();
            xml = stringWriter.GetStringBuilder().ToString();

            return xml;
        }

        public bool VerifySignature(string xml)
        {
            _logger.Log("checking iDx signature...");
            var xmldocument = new XmlDocument { PreserveWhitespace = true };
            xmldocument.LoadXml(xml);

            var signatures = xmldocument.GetElementsByTagName("Signature", "*");

            var signature = (XmlElement)signatures[signatures.Count - 1];
            if (!CheckIdxSignature(xmldocument, signature))
            {
                return false;
            }

            if (signatures.Count == 2)
            {
                _logger.Log("checking SAML message signature...");
                bool isValidSignature;
                (new BankIdSignature(_configuration)).TryVerifyElement(xml, "Assertion", "urn:oasis:names:tc:SAML:2.0:assertion", out isValidSignature);

                return isValidSignature;
            }

            return true;
        }

        private bool CheckIdxSignature(XmlDocument document, XmlElement signature)
        {
            var signedXml = new SignedXml(document);
            signedXml.LoadXml(signature);

            X509Certificate2 incomingCertificate = null;

            foreach (var keyInfo in signedXml.KeyInfo.OfType<KeyInfoName>())
            {
                var keyInfoValue = keyInfo.Value;

                if (!string.IsNullOrEmpty(keyInfoValue))
                {
                    if (string.Compare(keyInfoValue, _configuration.RoutingServiceCertificate.Thumbprint, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        incomingCertificate = _configuration.RoutingServiceCertificate;
                        break;
                    }
                    if (string.Compare(keyInfoValue, _configuration.AlternateRoutingServiceCertificate?.Thumbprint, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        incomingCertificate = _configuration.AlternateRoutingServiceCertificate;
                        break;
                    }
                }
            }

            if (incomingCertificate == null)
            {
                _logger.Log("the certificate used for signing is not the same as the one in the configuration");
                return false;
            }

            if (!signedXml.CheckSignature(incomingCertificate, true))
            {
                _logger.Log("signature is not valid");
                return false;
            }

            _logger.Log("signature is valid");
            return true;
        }

        public bool VerifySchema(string xml)
        {
            var xDoc = XDocument.Parse(xml);
            var root = xDoc.Root;

            foreach (var tuple in TypeSchemaMapping)
            {
                Debug.Assert(root != null, "root != null");

                if (tuple.Key.Name != root.Name.LocalName) continue;

                var xmlRoot = (XmlTypeAttribute)Attribute.GetCustomAttribute(tuple.Key, typeof(XmlTypeAttribute));
                root.Validate(SchemaSet.GlobalElements[new XmlQualifiedName(tuple.Key.Name, xmlRoot.Namespace)], SchemaSet, null);
            }

            return true;
        }
    }
}
