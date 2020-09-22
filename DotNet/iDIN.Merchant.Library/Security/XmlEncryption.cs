using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace BankId.Merchant.Library.Security
{
    internal class XmlEncryption
    {
        internal static void DecryptXml(RSA asymmetricAlgorithm, XmlDocument xmlDoc, string[] xmlElementsXPaths)
        {
            if (asymmetricAlgorithm == null) throw new ArgumentNullException("asymmetricAlgorithm");
            if (xmlDoc == null) throw new ArgumentNullException("xmlDoc");
            if (xmlElementsXPaths == null) throw new ArgumentNullException("xmlElementsXPaths");

            // create the symmetric algorithm which was used for encryption
            var symmetricAlgorithm = new AesManaged();
            symmetricAlgorithm.Padding = PaddingMode.ISO10126;

            foreach (var xPath in xmlElementsXPaths)
            {
                // select all encrypted attribute elements
                var encryptedElements = xmlDoc.SelectNodes(xPath);

                Debug.Assert(encryptedElements != null, "encryptedElements != null");
                foreach (XmlNode encryptedElement in encryptedElements)
                {
                    // load the encrypted data element
                    var encryptedDataElement = encryptedElement.SelectSingleNode("//*[local-name() = 'EncryptedData']") as XmlElement;
                    var encryptedData = new EncryptedData();
                    Debug.Assert(encryptedDataElement != null, "encryptedDataElement != null");
                    encryptedData.LoadXml(encryptedDataElement);

                    // load the encrypted key element
                    var encryptedKeyElement = encryptedDataElement.SelectSingleNode("//*[local-name() = 'EncryptedKey']") as XmlElement;
                    var encryptedKey = new EncryptedKey();
                    Debug.Assert(encryptedKeyElement != null, "encryptedKeyElement != null");
                    encryptedKey.LoadXml(encryptedKeyElement);

                    // decrypt the key using the specifief asymmetric algorithm
                    var symetricKey = asymmetricAlgorithm.Decrypt(encryptedKey.CipherData.CipherValue, RSAEncryptionPadding.OaepSHA1);

                    // use the asymmetric decrypted key to decrypt the encrypted data using the specified symmetric algorithm
                    symmetricAlgorithm.Key = symetricKey;

                    var output = new EncryptedXml { Mode = CipherMode.CBC, Padding = PaddingMode.ISO10126 };
                    var data = output.DecryptData(encryptedData, symmetricAlgorithm);

                    // replace the encrypted element with its decrypted form
                    output.ReplaceData((XmlElement)encryptedElement, data);
                }
            }
        }
    }
}