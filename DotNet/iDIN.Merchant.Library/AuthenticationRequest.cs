using System;
using System.Text.RegularExpressions;
using BankId.Merchant.Library.Helpers;

namespace BankId.Merchant.Library
{
    /// <summary>
    /// Describes a new authentication request
    /// </summary>
    public class AuthenticationRequest
    {
        private static readonly TimeSpan MaxExpirationPeriod = TimeSpan.FromMinutes(5);
        private static readonly Regex MerchantReferenceFormat = new Regex("^[a-zA-Z][a-zA-Z0-9]{0,34}$");


        /// <summary>
        /// Constructor that highlights all required fields for this object; use this one to specify your own messageId
        /// </summary>
        public AuthenticationRequest(string entranceCode, ServiceIds requestedServiceId, string issuerId, string merchantReference = null,
            AssuranceLevel assuranceLevel = AssuranceLevel.Loa3, TimeSpan? expirationPeriod = null, string language = "nl", string merchantDocumentId = null)
        {
            if (expirationPeriod > MaxExpirationPeriod)
                throw new ArgumentOutOfRangeException(nameof(expirationPeriod), "The expiration period cannot be greater than five minutes.");

            if (merchantReference != null && !MerchantReferenceFormat.IsMatch(merchantReference))
                throw new ArgumentException("MerchantReference does not follow expected format - " + MerchantReferenceFormat, nameof(merchantReference));

            if (!String.IsNullOrEmpty(merchantDocumentId))
            {
                if ((requestedServiceId & ServiceIds.Sign) != 0)
                {
                    if ((requestedServiceId & ServiceIds.ConsumerBin) == 0)
                    {
                        throw new ArgumentException("ConsumerID BIN attribute should be present.");
                    }
                }
                else if (requestedServiceId != ServiceIds.Sign)
                {
                    throw new ArgumentException("DocumentID should not be filled if the Sign service is not requested.");
                }
            }
            else
            {
                if (requestedServiceId == ServiceIds.Sign || (requestedServiceId & ServiceIds.Sign) != 0)
                {
                    throw new ArgumentException("DocumentID should be present.");
                }
            }

            EntranceCode = entranceCode;
            MerchantReference = merchantReference ?? GenerateMerchantReference();
            Language = language;
            RequestedServiceId = requestedServiceId;
            IssuerId = issuerId;
            AssuranceLevel = assuranceLevel;
            ExpirationPeriod = expirationPeriod;
            DocumentId = merchantDocumentId;
        }

        /// <summary>
        /// An 'authentication identifier' to facilitate continuation of the session even if the existing session has been lost.
        /// </summary>
        public string EntranceCode { get; private set; }

        /// <summary>
        /// This field is the unique authentication reference from the Merchant.
        /// </summary>
        public string MerchantReference { get; private set; }

        /// <summary>
        /// This field enables the Issuers's site to select the Consumer's preferred language (e.g. the language selected on the Merchant's site),
        /// if the Issuers's site supports this: Dutch = 'nl', English = 'en'.
        /// Language used by Consumer formatted like ISO 639-1. 
        /// </summary>
        public string Language { get; private set; }

        /// <summary>
        /// Optional: The period of validity of the authentication request
        /// </summary>
        public TimeSpan? ExpirationPeriod { get; private set; }

        /// <summary>
        /// This field specifies what attributes the Merchant would like to have a look at.
        /// </summary>
        public ServiceIds RequestedServiceId { get; private set; }

        /// <summary>
        /// BIC of the Issuer
        /// </summary>
        public string IssuerId { get; private set; }

        /// <summary>
        /// This fiels specified the level of assurance for authentication of Consumer.
        /// </summary>
        public AssuranceLevel AssuranceLevel { get; private set; }

        /// <summary>
        /// DocumentId to be signed
        /// </summary>
        public string DocumentId { get; private set; }

        private DateTime? _createDateTimestamp;
        /// <summary>
        /// Represents the current time at which this authentication message is created.
        /// </summary>
        public DateTime CreateDateTimestamp
        {
            get
            {
                if (!_createDateTimestamp.HasValue)
                    _createDateTimestamp = DateTimeHelper.Now();

                return _createDateTimestamp.Value;
            }
        }

        private static string GenerateMerchantReference()
        {
            return ('A' + Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N")).Substring(0, 35);
        }
    }
}
