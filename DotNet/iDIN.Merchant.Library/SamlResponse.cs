using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using BankId.Merchant.Library.Security;
using BankId.Merchant.Library.Xml.Schemas.Saml.Protocol;

namespace BankId.Merchant.Library
{
    /// <summary>
    /// Class responsible for handling Saml Responses
    /// </summary>
    public class SamlResponse
    {
        private SamlResponse(ResponseType responseType)
        {
            if (responseType == null) throw new ArgumentNullException("responseType");

            TransactionId = responseType.ID;
            MerchantReference = responseType.InResponseTo;
            Version = responseType.Version;
            AcquirerId = responseType.Issuer.Value;

            if (responseType.Status != null)
            {
                if (responseType.Status.StatusCode.StatusCode == null)
                {
                    throw new CommunicatorException("Missing second level status code");
                }
                Status = new SamlStatus(responseType.Status.StatusMessage, responseType.Status?.StatusCode?.Value, responseType.Status?.StatusCode?.StatusCode?.Value);                
            }

            if (responseType.Items == null)
            {
                AttributeStatements = new ReadOnlyCollection<SamlAttribute>(new Collection<SamlAttribute>());
                return;
            }

            var attributeStatements = new Collection<SamlAttribute>();

            // extract Consumer.BIN attribute
            if (responseType.Items.Length > 0)
            {
                var itemsField = ((AssertionType)responseType.Items[0]).Subject.Items;
                if (itemsField != null && itemsField.Length > 0)
                {
                    var value = ((NameIDType)itemsField[0]).Value;
                    if (value.StartsWith("TRANS"))
                    {
                        attributeStatements.Add(new SamlAttribute(SamlAttribute.ConsumerTransientID, value));
                    }
                    else
                    {
                        attributeStatements.Add(new SamlAttribute(SamlAttribute.ConsumerBin, value));
                    }
                }
            }

            // extract attribute values
            var assertionTypes = responseType.Items.Where(x => x.GetType().Name == "AssertionType").Select(x => (AssertionType)x).ToList();

            if (!assertionTypes.Any())
            {
                AttributeStatements = new ReadOnlyCollection<SamlAttribute>(attributeStatements);
                return;
            }

            var attributeStatementTypes =
                assertionTypes.SelectMany(x => x.Items)
                    .Where(x => x.GetType().Name == "AttributeStatementType")
                    .Select(x => (AttributeStatementType)x);

            var attributeTypes =
                attributeStatementTypes.SelectMany(x => x.Items)
                    .Where(x => x.GetType().Name == "AttributeType")
                    .Select(x => (AttributeType)x);
            
            foreach (var attributeType in attributeTypes)
            {
                var values =
                    attributeType.AttributeValue.Where(x => x.GetType().Name == "XmlNode[]")
                        .SelectMany(x => (XmlNode[])x).Where(x => x.NodeType == XmlNodeType.Text)
                        .Select(x => x.Value);
                
                var value = string.Concat(values);
                if (string.IsNullOrEmpty(value) && attributeType.AttributeValue.Length > 0)
                {
                    var attributeValue = attributeType.AttributeValue.FirstOrDefault();
                    if (attributeValue?.GetType() != typeof(object))
                    {
                        value = attributeValue?.ToString();
                    }
                }

                attributeStatements.Add(new SamlAttribute(attributeType.Name, value));
            }          

            AttributeStatements = new ReadOnlyCollection<SamlAttribute>(attributeStatements);
        }

        /// <summary>
        /// The Transaction ID
        /// </summary>
        public string TransactionId { get; private set; }

        /// <summary>
        /// Unique transaction reference fron the Merchant
        /// </summary>
        public string MerchantReference { get; private set; }

        /// <summary>
        /// The SAML Version
        /// </summary>
        public string Version { get; private set; }
        /// <summary>
        /// Represents the current time at which this SAML Response message is created.
        /// </summary>
        public DateTime CreateDateTimestamp { get; private set; }
        /// <summary>
        /// The Acquirer ID
        /// </summary>
        public string AcquirerId { get; private set; }

        /// <summary>
        /// The SAML Attributes required by the merchant
        /// </summary>
        public IReadOnlyCollection<SamlAttribute> AttributeStatements { get; }


        /// <summary>
        /// Details of the SAML status.
        /// </summary>
        public SamlStatus Status { get; private set; }

        /// <summary>
        /// Gets attribute value for the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetAttributeValue(string name)
        {
            var attribute = AttributeStatements.FirstOrDefault(x => x.Name == name);
            return attribute?.Value;
        }

        internal static SamlResponse Parse(string xml, IConfiguration configuration)
        {
            var samlXml = new XmlDocument();
            samlXml.LoadXml(xml);

            try
            {
                XmlEncryption.DecryptXml(
                    configuration.SamlCertificate.GetRSAPrivateKey(),
                    samlXml,
                    new[] { "//*[local-name() = 'EncryptedID']", "//*[local-name() = 'EncryptedAttribute']" });
            }
            catch (Exception e)
            {
                throw new CommunicatorException("Error decrypting data. See inner exception for more details.", e);
            }

            var doc = ResponseType.Deserialize(samlXml.InnerXml);

            return new SamlResponse(doc);
        }
    }
}
