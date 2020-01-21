using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BankId.Merchant.Library
{
    internal static class SamlAttributeValidator
    {
        private static readonly Dictionary<string, string> ValidationPatterns = new Dictionary<string, string>();
        private static readonly Dictionary<string, Regex> RegexPatternsCache = new Dictionary<string, Regex>();

        static SamlAttributeValidator()
        {
            ValidationPatterns.Add(SamlAttribute.ConsumerBin, @"^[A-Za-z]{2}[\P{Cc}]{0,1024}$");
            ValidationPatterns.Add(SamlAttribute.ConsumerTransientID, @"^(?i)TRANS(?-i)[\x20-\x7E]{1,251}$");
            ValidationPatterns.Add(SamlAttribute.ConsumerDeprecatedBin, @"^[A-Za-z]{2}[\P{Cc}]{0,1024}$");
            ValidationPatterns.Add(SamlAttribute.ConsumerGender, @"^0|1|2|9$");
            ValidationPatterns.Add(SamlAttribute.ConsumerLegalLastName, @"^[\P{Cc}]{1,200}$");
            ValidationPatterns.Add(SamlAttribute.ConsumerLegalLastNamePrefix, @"^[\P{Cc}]{1,10}$");
            ValidationPatterns.Add(SamlAttribute.ConsumerPrefLastName, @"^[\P{Cc}]{1,200}$");
            ValidationPatterns.Add(SamlAttribute.ConsumerPrefLastNamePrefix, @"^[\P{Cc}]{1,10}$");
            ValidationPatterns.Add(SamlAttribute.ConsumerPartnerLastName, @"^[\P{Cc}]{1,200}$");
            ValidationPatterns.Add(SamlAttribute.ConsumerPartnerLastNamePrefix, @"^[\P{Cc}]{1,10}$");
            ValidationPatterns.Add(SamlAttribute.ConsumerInitials, @"^[\p{Lu}]{1,24}$");
            ValidationPatterns.Add(SamlAttribute.ConsumerDateOfBirth, @"^[\d]{4}(0[1-9]|1[012]|00)(0[1-9]|[12][0-9]|3[01]|00)$");
            ValidationPatterns.Add(SamlAttribute.ConsumerStreet, @"^[\P{Cc}]{1,43}$");
            ValidationPatterns.Add(SamlAttribute.ConsumerHouseNo, @"^[0-9]{1,5}$");
            ValidationPatterns.Add(SamlAttribute.ConsumerAddressExtra, @"^[\P{Cc}]{1,70}$");
            ValidationPatterns.Add(SamlAttribute.ConsumerAddressLine1, @"^[\P{Cc}]{1,70}$");
            ValidationPatterns.Add(SamlAttribute.ConsumerAddressLine2, @"^[\P{Cc}]{1,70}$");
            ValidationPatterns.Add(SamlAttribute.ConsumerAddressLine3, @"^[\P{Cc}]{1,70}$");
            ValidationPatterns.Add(SamlAttribute.ConsumerPostalCode, @"^[0-9]{4}[a-zA-Z]{2}$");
            ValidationPatterns.Add(SamlAttribute.ConsumerCity, @"^[\P{Cc}]{1,24}$");
            ValidationPatterns.Add(SamlAttribute.ConsumerCountry, @"^[a-zA-Z]{2}$");
            ValidationPatterns.Add(SamlAttribute.ConsumerIs18OrOlder, @"^true|false$");
            ValidationPatterns.Add(SamlAttribute.Telephone, @"^[\d\s\+\-\(\)]{1,20}$");
            ValidationPatterns.Add(SamlAttribute.Email, @"^[\P{Cc}]{1,255}$");

            InitializeRegexCache();
        }

        private static void InitializeRegexCache()
        {
            foreach (var key in ValidationPatterns.Keys)
            {
                RegexPatternsCache.Add(key, new Regex(ValidationPatterns[key]));
            }
        }

        internal static void Validate(SamlAttribute attribute)
        {
            if (attribute == null) throw new ArgumentNullException("attribute");

            if (!RegexPatternsCache.ContainsKey(attribute.Name))
                return;

            var regex = RegexPatternsCache[attribute.Name];
            var match = regex.Match(attribute.Value);
            if (!match.Success)
            {
                throw new Exception(string.Format("Saml attribute value not valid ({0} : {1}).", attribute.Name, attribute.Value));
            }
        }
    }
}
