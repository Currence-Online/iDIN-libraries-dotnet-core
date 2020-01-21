using System;
using System.Collections.Generic;
using System.Xml;
using BankId.Merchant.Library.Helpers;
using BankId.Merchant.Library.Xml.Schemas.Saml.Protocol;

namespace BankId.Merchant.Library.MessageBuilders
{
    internal class BankIdMessageBuilder
    {
        private readonly IConfiguration _configuration;
        private readonly string[] _dateTimeElementNames = { "AuthnRequest@IssueInstant" };

        public BankIdMessageBuilder(IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            _configuration = configuration;
        }

        public string GetNewTransaction(AuthenticationRequest authenticationRequest)
        {
            if (authenticationRequest == null) throw new ArgumentNullException("authenticationRequest");

            var authnRequest = new AuthnRequestType
            {
                ID = authenticationRequest.MerchantReference,
                Version = Constants.BankIdAuthnRequestTypeVersion,
                IssueInstant = authenticationRequest.CreateDateTimestamp,
                ForceAuthn = true,                
                IsPassive = false,
                ProtocolBinding = Constants.BankIdProtocolBinding,
                AssertionConsumerServiceURL = _configuration.MerchantReturnUrl.AbsoluteUri,                
                AttributeConsumingServiceIndex = (ushort)authenticationRequest.RequestedServiceId,
                AttributeConsumingServiceIndexSpecified = true,
                Issuer = new NameIDType
                {
                    Value = _configuration.MerchantId                                        
                },                
                Conditions = new ConditionsType(),
                RequestedAuthnContext = new RequestedAuthnContextType
                {
                    Comparison = AuthnContextComparisonType.minimum,     
                    ComparisonSpecified = true,
                    ItemsElementName = new[] { ItemsChoiceType7.AuthnContextClassRef },
                    Items = new[] { GetLevelOfAssuranceString(authenticationRequest.AssuranceLevel) }
                },       
                Extensions = GetAuthnRequestExtensions(authenticationRequest),
                Scoping = new ScopingType()
            };                                   

            return DateTimeHelper.ProcessDateTimes(authnRequest.Serialize(), _dateTimeElementNames);
        }

        private static string GetLevelOfAssuranceString(AssuranceLevel assuranceLevel)
        {            
            switch (assuranceLevel)
            {
                case AssuranceLevel.Loa3:
                    return Constants.AssuranceLevel3;
                default:
                    throw new CommunicatorException(string.Format("Level of assurance not supported: {0}.", Enum.GetName(typeof(AssuranceLevel), assuranceLevel)));
            }            
        }    
        private static ExtensionsType GetAuthnRequestExtensions(AuthenticationRequest authenticationRequest)
        {
            ExtensionsType result = new ExtensionsType();

            List<System.Xml.XmlElement> resultList = new List<System.Xml.XmlElement>();

            if (!String.IsNullOrEmpty(authenticationRequest.DocumentId))
            {
                var documentAttribute = new BankId.Merchant.Library.Xml.Schemas.Saml.Assertion.AttributeType(){
                    Name = SamlAttribute.DocumentId,
                    AttributeValue = new object[] { authenticationRequest.DocumentId }
                };

                resultList.Add(GetXmlElement(documentAttribute.Serialize()));
            }

            if (resultList.Count == 0)
                return null;

            result.Any = resultList.ToArray();

            return result;
        }
        private static XmlElement GetXmlElement(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.DocumentElement;
        }
    }
}
