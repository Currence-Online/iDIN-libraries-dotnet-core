using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace BankId.Merchant.Library.Security
{
    internal class BankIdSignature
    {
        private readonly ILogger _logger;

        internal BankIdSignature(IConfiguration configuration)
        {
            XmlSignature.RegisterSignatureAlghorighm();
            _logger = configuration.GetLogger();
        }

        private static bool IsEligibleForBankIdSignature(XmlDocument doc)
        {
            // only responses with a StatusCode value of Success should be signed
            // (meaning only those with <samlp:StatusCode Value="urn:oasis:names:tc:SAML:2.0:status:Success"/>)
            var element = doc.SelectSingleNode("//*[local-name()='StatusCode']") as XmlElement;
            if (element != null)
            {
                if (element.GetAttribute("Value") == "urn:oasis:names:tc:SAML:2.0:status:Success")
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to verify the specified XML text signature.
        /// </summary>
        /// <param name="xmlText">The XML text.</param>
        /// <param name="elementName">The name of the element signature to be verified.</param>
        /// <param name="elementNamespace">The namespace of the element signature to be verified.</param>
        /// <param name="isValidSignature">True if the signature is valid and placed properly, false otherwise.</param>
        /// <returns>True if the verifying was possible, false otherwise.</returns>
        public bool TryVerifyElement(string xmlText, string elementName, string elementNamespace, out bool isValidSignature)
        {
            _logger.Log("Debug: TryVerifyElement, xml={0}, elname={1}", xmlText, elementName);

            isValidSignature = false;

            if (string.IsNullOrEmpty(elementName))
            {
                _logger.Log("Debug: The element name is empty. There is no element to verify the signature for.");
                return false;
            }

            var xmlDoc = new XmlDocument { PreserveWhitespace = true };
            xmlDoc.LoadXml(xmlText);

            var elements = xmlDoc.GetElementsByTagName(elementName, elementNamespace);
            if (elements.Count == 0)
            {
                _logger.Log("Debug: Cannot verify the signature of the '{0}' element. No '{0}' element was found in the document.", elementName);
                return false;
            }

            // not eligible to have signature
            if (!IsEligibleForBankIdSignature(xmlDoc))
                throw new CommunicatorException("Response should not have a BankId signature.");

            var elementToSign = elements[0] as XmlElement;

            var xmlElementDoc = new XmlDocument { PreserveWhitespace = true };
            Debug.Assert(elementToSign != null, "elementToSign != null");
            xmlElementDoc.LoadXml(elementToSign.OuterXml);

            // the signature should have been placed inside the 'Assertion' element (more specific, right after the <Issuer> element)
            var element = GetBankIdSignatureElement(xmlElementDoc.DocumentElement);
            if (element == null)
            {
                _logger.Log("Debug: No 'Signature' element was found in the document at the expected location.", elementName);
                return false;
            }

            var keyInfo = XmlSignature.GetElementUnderRoot(element, "KeyInfo");
            var x509Data = XmlSignature.GetElementUnderRoot(keyInfo as XmlElement, "X509Data");
            var x509Certificate = XmlSignature.GetElementUnderRoot(x509Data as XmlElement, "X509Certificate");

            var certificate = Convert.FromBase64String(x509Certificate.InnerText);
            var cert = new X509Certificate2(certificate);

            isValidSignature = XmlSignature.CheckSignature(xmlElementDoc, cert, GetBankIdSignatureElement(xmlElementDoc.DocumentElement));
            _logger.Log("Debug: TryVerifyElement, isvalid={0}", isValidSignature);
            return true;
        }

        private static XmlElement GetBankIdSignatureElement(XmlElement xmlElement)
        {
            var signatureElement = XmlSignature.GetElementUnderRoot(xmlElement, "Signature", "http://www.w3.org/2000/09/xmldsig#") as XmlElement;

            return signatureElement;
        }
    }
}