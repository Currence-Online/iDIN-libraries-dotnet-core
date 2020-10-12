using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;
using BankId.Merchant.Library.Tests.Utilities;
using Shouldly;
using Xunit;

namespace BankId.Merchant.Library.Tests
{
    public class AttributeEncryptionKeysShould : BaseTest
    {
        [Fact]
        public void GenerateCorrectRequest()
        {
            var response = Invoke("StatusResponse-Sample");

            response.IsError.ShouldBe(false);
            response.SamlResponse.ShouldNotBe(null);
            response.Status.ShouldBe("Success");
        }

        [Fact]
        public void Parse_the_attributes()
        {
            var response = Invoke("StatusResponse-Sample");

            response.IsError.ShouldBe(false);
            response.SamlResponse.ShouldNotBe(null);

            response.SamlResponse.AttributeStatements.Count.ShouldBe(9);
            response.SamlResponse.GetAttributeValue(SamlAttribute.ConsumerBin).ShouldBe("Some Subject");
            response.SamlResponse.GetAttributeValue(SamlAttribute.ConsumerIs18OrOlder).ShouldBe(null);
            response.SamlResponse.GetAttributeValue(SamlAttribute.ConsumerLegalLastName).ShouldBe("Smith");
            response.SamlResponse.GetAttributeValue(SamlAttribute.ConsumerPrefLastName).ShouldBe("John");
            response.SamlResponse.GetAttributeValue(SamlAttribute.ConsumerPartnerLastName).ShouldBe("Jane");
            response.SamlResponse.GetAttributeValue(SamlAttribute.ConsumerLegalLastNamePrefix).ShouldBe("Sm");
            response.SamlResponse.GetAttributeValue(SamlAttribute.ConsumerPrefLastNamePrefix).ShouldBe("Jo");
            response.SamlResponse.GetAttributeValue(SamlAttribute.ConsumerPartnerLastNamePrefix).ShouldBe("Ja");
            response.SamlResponse.GetAttributeValue(SamlAttribute.ConsumerInitials).ShouldBe("SJĆ");
            response.SamlResponse.GetAttributeValue(SamlAttribute.DeliveredServiceID).ShouldBe("4096");
        }

        [Fact]
        public void RetrieveAttributesEncryptionKeys()
        {
            var response = Invoke("StatusResponse-Sample");

            response.SamlResponse.AttributesEncryptionKeys.ShouldNotBe(null);
            response.SamlResponse.AttributesEncryptionKeys.Count.ShouldBe(8);
        }

        [Fact]
        public void RecomputeSubjectBasedOnAttributesEncryptionKeys()
        {
            var response = Invoke("StatusResponse-Sample");
            var expected = "Some Subject";

            response.SamlResponse.GetAttributeValue(SamlAttribute.ConsumerBin).ShouldBe(expected);
            var attribute = response.SamlResponse.AttributesEncryptionKeys.FirstOrDefault(x => x.AttributeName == SamlAttribute.ConsumerBin);
            attribute.ShouldNotBe(null);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response.RawMessage);
            var result = DecryptAttributeElement(xmlDoc, 0, attribute.AesKey);

            result.ShouldBe(expected);
        }

        [Fact]
        public void RecomputeAttributeValueBasedOnAttributesEncryptionKeys()
        {
            var response = Invoke("StatusResponse-Sample");
            var expected = "John";

            response.SamlResponse.GetAttributeValue(SamlAttribute.ConsumerPrefLastName).ShouldBe(expected);
            var attribute = response.SamlResponse.AttributesEncryptionKeys.FirstOrDefault(x => x.AttributeName == SamlAttribute.ConsumerPrefLastName);
            attribute.ShouldNotBe(null);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response.RawMessage);

            var result = DecryptAttributeElement(xmlDoc, 2, attribute.AesKey);
            result.ShouldBe(expected);
        }

        [Fact]
        public void RecomputeAttributeValuesBasedOnAttributesEncryptionKeys()
        {
            var response = Invoke("StatusResponse-Sample");
            var attributesEncryptionKeys = response.SamlResponse.AttributesEncryptionKeys;

            attributesEncryptionKeys.Count.ShouldBe(8);

            for (int i = 0; i < attributesEncryptionKeys.Count; i++)
            {
                var samlAttributesEncryptionKey = attributesEncryptionKeys.ElementAt(i);
                var samlAttributeExpected = response.SamlResponse.AttributeStatements.FirstOrDefault(att => att.Name == samlAttributesEncryptionKey.AttributeName);

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(response.RawMessage);

                var result = DecryptAttributeElement(xmlDoc, i, samlAttributesEncryptionKey.AesKey);
                result.ShouldBe(samlAttributeExpected.Value);
            }
        }

        private static string DecryptAttributeElement(XmlDocument xmlDoc, int attributeIndex, byte[] symetricKey)
        {
            var xmlElementsXPaths = new[] { "//*[local-name() = 'EncryptedID']", "//*[local-name() = 'EncryptedAttribute']" };

            var attributeCount = 0;

            foreach (var xmlElementsXPath in xmlElementsXPaths)
            {
                var encryptedElements = xmlDoc.SelectNodes(xmlElementsXPath);
                if (encryptedElements == null)
                {
                    continue;
                }

                foreach (XmlNode encryptedElement in encryptedElements)
                {
                    if (attributeIndex != attributeCount)
                    {
                        attributeCount++;
                    }
                    else
                    {
                        var encryptedDataElement = encryptedElement.Clone().SelectSingleNode("//*[local-name() = 'EncryptedData']") as XmlElement;
                        var encryptedData = new EncryptedData();
                        encryptedData.LoadXml(encryptedDataElement);

                        // use the asymmetric decrypted key to decrypt the encrypted data using the specified symmetric algorithm
                        // create the symmetric algorithm which was used for encryption
                        var symmetricAlgorithm = new AesManaged();

                        symmetricAlgorithm.Padding = PaddingMode.ISO10126;

                        symmetricAlgorithm.Key = symetricKey;

                        var output = new EncryptedXml { Mode = CipherMode.CBC, Padding = PaddingMode.ISO10126 };

                        var data = output.DecryptData(encryptedData, symmetricAlgorithm);

                        XmlDocument docElement = new XmlDocument();
                        MemoryStream ms = new MemoryStream(data);
                        docElement.Load(ms);
                        return docElement.InnerText;
                    }
                }
            }

            return "";
        }
        private StatusResponse Invoke(string message)
        {
            messenger = new TestMessenger(TestMessage.Get(message));
            SetupConfiguration();
            mockConfig.Setup(x => x.MerchantSubId).Returns(42);

            var communicator = new Communicator(mockConfig.Object);
            return communicator.GetResponse(new StatusRequest("1234000000002144"));
        }
    }
}
