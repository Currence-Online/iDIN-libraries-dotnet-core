using System;

namespace BankId.Merchant.Library
{
    /// <summary>
    /// Class responsible for handling Saml Attributes
    /// </summary>
    public class SamlAttribute
    {
        /// <summary>
        /// Get the delivered service id (can be the same as RequestedServiceID, a different number, or 0) 
        /// </summary>
        public static readonly string DeliveredServiceID = "urn:nl:bvn:bankid:1.0:bankid.deliveredserviceid";
        /// <summary>
        /// Get the BIN(Bank Identifying Number) 
        /// </summary>
        public static readonly string ConsumerBin = "urn:nl:bvn:bankid:1.0:consumer.bin";
        /// <summary>
        /// Get the Consumer TransientID
        /// </summary>
        public static readonly string ConsumerTransientID = "urn:nl:bvn:bankid:1.0:consumer.transientid";
        /// <summary>
        /// Get the Consumer gender
        /// </summary>
        public static readonly string ConsumerGender = "urn:nl:bvn:bankid:1.0:consumer.gender";
        /// <summary>
        /// Get the Consumer legal last name 
        /// </summary>
        public static readonly string ConsumerLegalLastName = "urn:nl:bvn:bankid:1.0:consumer.legallastname";
        /// <summary>
        /// Get the Consumer preferred last name
        /// </summary>
        public static readonly string ConsumerPrefLastName = "urn:nl:bvn:bankid:1.0:consumer.preferredlastname";
        /// <summary>
        /// Get the Consumer's registered partner last name (analogous to legal last name)
        /// </summary>
        public static readonly string ConsumerPartnerLastName = "urn:nl:bvn:bankid:1.0:consumer.partnerlastname";
        /// <summary>
        /// Get the last name prefix of Consumer's legal last name
        /// </summary>
        public static readonly string ConsumerLegalLastNamePrefix = "urn:nl:bvn:bankid:1.0:consumer.legallastnameprefix";
        /// <summary>
        /// Get the last name prefix of Consumer preferred last name (analogous to legal last name prefix)
        /// </summary>
        public static readonly string ConsumerPrefLastNamePrefix = "urn:nl:bvn:bankid:1.0:consumer.preferredlastnameprefix";
        /// <summary>
        /// Get the last name prefix of Consumer partner last name (analogous to legal last name prefix)
        /// </summary>
        public static readonly string ConsumerPartnerLastNamePrefix = "urn:nl:bvn:bankid:1.0:consumer.partnerlastnameprefix";
        /// <summary>
        /// Get the initials of the Consumer
        /// </summary>
        public static readonly string ConsumerInitials = "urn:nl:bvn:bankid:1.0:consumer.initials";
        /// <summary>
        /// Get the date of birth of the Consumer
        /// </summary>
        public static readonly string ConsumerDateOfBirth = "urn:nl:bvn:bankid:1.0:consumer.dateofbirth";
        /// <summary>
        /// Get the street name of the Consumer's residential address
        /// </summary>
        public static readonly string ConsumerStreet = "urn:nl:bvn:bankid:1.0:consumer.street";
        /// <summary>
        /// Get the house number of the Consumer's residential address
        /// </summary>
        public static readonly string ConsumerHouseNo = "urn:nl:bvn:bankid:1.0:consumer.houseno";
        /// <summary>
        /// Get the house number suffix of the Consumer's residential address. Used for NL addresses only
        /// </summary>
        public static readonly string ConsumerHouseNoSuf = "urn:nl:bvn:bankid:1.0:consumer.housenosuf";
        /// <summary>
        /// Get additional address details of Consumer's residential address. Used for NL addresses only
        /// </summary>
        public static readonly string ConsumerAddressExtra = "urn:nl:bvn:bankid:1.0:consumer.addressextra";
        /// <summary>
        /// Get the first address line of Consumer's residential address
        /// </summary>
        public static readonly string ConsumerAddressLine1 = "urn:nl:bvn:bankid:1.0:consumer.intaddressline1";
        /// <summary>
        /// Get the second address line of Consumer's residential address
        /// </summary>
        public static readonly string ConsumerAddressLine2 = "urn:nl:bvn:bankid:1.0:consumer.intaddressline2";
        /// <summary>
        /// Get the third address line of Consumer's residential address
        /// </summary>
        public static readonly string ConsumerAddressLine3 = "urn:nl:bvn:bankid:1.0:consumer.intaddressline3";
        /// <summary>
        /// Get the postal code of the Consumer's residential address
        /// </summary>
        public static readonly string ConsumerPostalCode = "urn:nl:bvn:bankid:1.0:consumer.postalcode";
        /// <summary>
        /// Get the city of the Customer's residential address
        /// </summary>
        public static readonly string ConsumerCity = "urn:nl:bvn:bankid:1.0:consumer.city";
        /// <summary>
        /// Get the country code of the country where the Consumer resides
        /// </summary>
        public static readonly string ConsumerCountry = "urn:nl:bvn:bankid:1.0:consumer.country";
        /// <summary>
        /// Get the Deprecated BIN
        /// </summary>
        public static readonly string ConsumerDeprecatedBin = "urn:nl:bvn:bankid:1.0:consumer.deprecatedbin";        
        /// <summary>
        /// Get the value specifying if the Consumer is 18 or older
        /// </summary>
        public static readonly string ConsumerIs18OrOlder = "urn:nl:bvn:bankid:1.0:consumer.is18orolder";

        /// <summary>
        /// Get the email of the Consumer
        /// </summary>
        public static readonly string Email = "urn:nl:bvn:bankid:1.0:consumer.email";

        /// <summary>
        /// Get the Telephone of Consumer
        /// </summary>
        public static readonly string Telephone = "urn:nl:bvn:bankid:1.0:consumer.telephone";

        /// <summary>
        /// Get the DocumentId to be signed
        /// </summary>
        public static readonly string DocumentId = "urn:nl:bvn:bankid:1.0:merchant.documentID";

        /// <summary>
        /// Name of the Saml Attribute
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Value of the Saml Attribute
        /// </summary>
        public string Value { get; private set; }

        internal SamlAttribute(string name, string value)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (value == null) throw new ArgumentNullException("value");

            Name = name;
            Value = value;

            SamlAttributeValidator.Validate(this);
        }
    }
}
