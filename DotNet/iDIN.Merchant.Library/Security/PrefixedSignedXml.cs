using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace BankId.Merchant.Library.Security
{
    internal class PrefixedSignedXml : SignedXml
    {
        private readonly string _prefixNs;

        public PrefixedSignedXml(string prefix, XmlDocument document)
            : base(document)
        {
            _prefixNs = prefix;
        }

        public PrefixedSignedXml(string prefix, XmlElement element)
            : base(element)
        {
            _prefixNs = prefix;
        }

        public PrefixedSignedXml(string prefix)
        {
            _prefixNs = prefix;
        }

        public new void ComputeSignature()
        {
            ComputeSignature(_prefixNs);
        }

        void ComputeSignature(string prefix)
        {
            BuildDigestedReferences();

            var signingKey = SigningKey;
            if (signingKey == null)
            {
                throw new CryptographicException("Cryptography_Xml_LoadKeyFailed");
            }

            if (SignedInfo.SignatureMethod == null)
            {
                if (!(signingKey is DSA))
                {
                    if (!(signingKey is RSA))
                    {
                        throw new CryptographicException("Cryptography_Xml_CreatedKeyFailed");
                    }
                    if (SignedInfo.SignatureMethod == null)
                    {
                        SignedInfo.SignatureMethod = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
                    }
                }
                else
                {
                    SignedInfo.SignatureMethod = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";
                }
            }
            var description = CryptoConfig.CreateFromName(SignedInfo.SignatureMethod) as SignatureDescription;
            if (description == null)
            {
                throw new CryptographicException("Cryptography_Xml_SignatureDescriptionNotCreated");
            }
            var hash = description.CreateDigest();
            if (hash == null)
            {
                throw new CryptographicException("Cryptography_Xml_CreateHashAlgorithmFailed");
            }
            
            GetC14NDigest(hash, prefix);
            m_signature.SignatureValue = description.CreateFormatter(signingKey).CreateSignature(hash);
        }

        public new XmlElement GetXml()
        {
            return GetXml(_prefixNs);
        }

        XmlElement GetXml(string prefix)
        {
            var e = base.GetXml();
            SetPrefix(prefix, e);
            return e;
        }

        //Invokes private method of SignedXml - SignedXml.BuildDigestedReferences
        private void BuildDigestedReferences()
        {
            var t = typeof(SignedXml);
            var m = t.GetMethod("BuildDigestedReferences", BindingFlags.NonPublic | BindingFlags.Instance);
            m.Invoke(this, new object[] { });
        }
                
        private void GetC14NDigest(HashAlgorithm hash, string prefix)
        {
            var document = new XmlDocument();
            document.PreserveWhitespace = true;

            var e = SignedInfo.GetXml();
            document.AppendChild(document.ImportNode(e, true));

            var canonicalizationMethodObject = SignedInfo.CanonicalizationMethodObject;
            SetPrefix(prefix, document.DocumentElement);
            canonicalizationMethodObject.LoadInput(document);

            canonicalizationMethodObject.GetDigestedOutput(hash);
        }

        private void SetPrefix(string prefix, XmlNode node)
        {
            foreach (XmlNode n in node.ChildNodes)
            {
                SetPrefix(prefix, n);
            }

            node.Prefix = prefix;
        }
    }
}
