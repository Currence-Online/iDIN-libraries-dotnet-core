using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text.RegularExpressions;
using System.Xml;

namespace BankId.Merchant.Library.Security
{
    internal class XmlSignature
    {
        // ReSharper disable once InconsistentNaming
        public const string XmlDigSignRSASHA256Namespace = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
        public static void RegisterSignatureAlghorighm()
        {
            CryptoConfig.AddAlgorithm(typeof(RSAPKCS1SHA256SignatureDescription), XmlDigSignRSASHA256Namespace);
        }

        /// <summary>
        /// The algorithm elements to be verified into a signature.
        /// Note that also namespace prefixes are taken into account.
        /// </summary>
        private static readonly Regex[] AlgorithmsRegexEnvelopedSignature = {
                                    new Regex("<(([^<>:]*):)?CanonicalizationMethod[^\\\">]+\"http://www\\.w3\\.org/2001/10/xml-exc-c14n#\""),
                                    new Regex("<(([^<>:]*):)?SignatureMethod[^\\\">]+\"http://www\\.w3\\.org/2001/04/xmldsig-more#rsa-sha256\""),
                                    new Regex("<(([^<>:]*):)?Transform[^\\\">]+\"http://www\\.w3\\.org/2000/09/xmldsig#enveloped-signature\""),
                                    new Regex("<(([^<>:]*):)?Transform[^\\\">]+\"http://www\\.w3\\.org/2001/10/xml-exc-c14n#\""),
                                    new Regex("<(([^<>:]*):)?DigestMethod[^\\\">]+\"http://www\\.w3\\.org/2001/04/xmlenc#sha256\"")
                                    };

        /// <summary>
        /// Signs a document.
        /// </summary>
        /// <param name="doc">The Xml document. It will be modified to contain the signature generated</param>
        /// <param name="certificate">The certificate to use for signing</param>
        /// <param name="signatureContainer">The xml element inside of which the signature will be added</param>
        /// <param name="dsPrefix">In case the 'dsPrefix' is sent, the generated signature element will contain this prefix;
        /// e.g if 'ds' is sent, the element will look like this: &lt;ds:Signature xmlns:ds="http://www.w3.org/2000/09/xmldsig#" &gt;...</param>       
        /// <param name="useFingerprint">If true the public key of the certificate will be replaced with the fingerprint in the signature</param>
        /// <returns>The Signature element</returns>
        public static XmlElement Sign(ref XmlDocument doc, X509Certificate2 certificate, XmlElement signatureContainer, string dsPrefix = "", bool useFingerprint = true)
        {
            doc.PreserveWhitespace = true;

            //bool elementSigning = xmlElement != null;
            var rsaKey = certificate.GetRSAPrivateKey();
            KeyInfo keyInfo = new KeyInfo();
            PrefixedSignedXml signedXml;
            string referenceUri = "";

            if (useFingerprint)
            {
                keyInfo.AddClause(new RSAKeyValue(rsaKey));
            }
            else
            {
                keyInfo.AddClause(new KeyInfoX509Data(certificate));
            }

            signedXml = new PrefixedSignedXml(dsPrefix, doc);
            signedXml.SigningKey = rsaKey;

            // Get the signature object from the SignedXml object.
            Signature signatureRef = signedXml.Signature;

            //Pass id of the element to be signed or "" to specify that all of the current XML document should be signed
            Reference reference = new Reference(referenceUri);

            reference.DigestMethod = "http://www.w3.org/2001/04/xmlenc#sha256";

            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();

            XmlDsigExcC14NTransform excC14NTransform = new XmlDsigExcC14NTransform();
            reference.AddTransform(env);
            reference.AddTransform(excC14NTransform);

            // Add the Reference object to the Signature object.
            signatureRef.SignedInfo.AddReference(reference);

            signatureRef.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            signatureRef.SignedInfo.SignatureMethod = XmlDigSignRSASHA256Namespace;
            signatureRef.KeyInfo = keyInfo;

            signedXml.ComputeSignature();
            XmlElement xmlSignature = signedXml.GetXml();

            // Append the signature element to the XML document (if any)
            if (signatureContainer != null)
            {
                signatureContainer.AppendChild(doc.ImportNode(xmlSignature, true));
            }

            string xml = doc.OuterXml;

            if (useFingerprint)
            {
                xml = ReplaceKeyWithFingerprint(certificate.Thumbprint, dsPrefix, xml);
            }

            //reload the xml document from customized xml source
            doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.LoadXml(xml);

            //TODO: todo cip!!! why the first element?
            //return the Signature Element
            return (XmlElement)doc.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#")[0];
        }

        private static string ReplaceKeyWithFingerprint(string fingerprintHex, string dsPrefix, string xml)
        {
            //replace KeyValue with KeyName containing the fingerprint of the signing certificate
            if (string.IsNullOrEmpty(dsPrefix))
            {
                var keyNameTag = string.Format("<KeyName>{0}</KeyName>", fingerprintHex.Replace("-", ""));
                xml = Regex.Replace(xml, "<KeyValue>.*</KeyValue>", keyNameTag);
            }
            else
            {
                var keyNameTag = string.Format("<{1}:KeyName>{0}</{1}:KeyName>", fingerprintHex.Replace("-", ""), dsPrefix);
                xml = Regex.Replace(xml, string.Format("<{0}:KeyValue>.*</{0}:KeyValue>", dsPrefix), keyNameTag);
            }
            return xml;
        }

        /// <summary>
        /// Verifies the specified XML document signature.
        /// </summary>
        /// <param name="xmlDocument">The XML document.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="signEl">The signature element.</param>
        ///<returns>True if the signature is valid and placed properly, false otherwise</returns>
        public static bool CheckSignature(XmlDocument xmlDocument, X509Certificate2 certificate, XmlElement signEl)
        {
            var rsaKey = certificate.GetRSAPublicKey();

            if (signEl == null || signEl.LocalName != "Signature")
            {
                return false;
            }

            var signedXml = new SignedXml(xmlDocument);
            signedXml.LoadXml(signEl);

            if (signedXml.CheckSignature(certificate, true))
            {
                return AlgorithmsRegexEnvelopedSignature.All(regex => regex.IsMatch(signEl.InnerXml));
            }

            return false;
        }

        public static XmlNode GetElementUnderRoot(XmlElement root, string localName, string namespaceUri = "")
        {
            if (root.HasChildNodes)
            {
                for (int i = 0; i < root.ChildNodes.Count; i++)
                {
                    if (root.ChildNodes[i].LocalName == localName && (String.IsNullOrEmpty(namespaceUri) || root.ChildNodes[i].NamespaceURI == namespaceUri))
                        return root.ChildNodes[i];
                }
            }
            return null;
        }
    }
}
