using System;
using BankId.Merchant.Library.Xml.Schemas.iDx;

namespace BankId.Merchant.Library
{
    /// <summary>
    /// Describes a new authentication response
    /// </summary>
    public class AuthenticationResponse : BaseResponse
    {
        /// <summary>
        /// The URL to redirect the Consumer for authentication
        /// </summary>
        public Uri IssuerAuthenticationUrl { get; private set; }

        /// <summary>
        /// The transaction ID
        /// </summary>
        public string TransactionId { get; private set; }

        /// <summary>
        /// DateTime set to when this transaction was created
        /// </summary>
        public DateTime TransactionCreateDateTimestamp { get; private set; }        

        private AuthenticationResponse(AcquirerTrxRes trxRes, string xml) : base(xml)
        {
            IssuerAuthenticationUrl = trxRes.Issuer.issuerAuthenticationURL.ParseAsUri(new CommunicatorException("Invalid AcquirerTrxRes.Issuer.issuerAuthenticationURL format."));
            TransactionId = trxRes.Transaction.transactionID;
            TransactionCreateDateTimestamp = trxRes.Transaction.transactionCreateDateTimestamp;
        }

        private AuthenticationResponse(AcquirerErrorRes errRes, string xml) : base(errRes, xml)
        {
            IssuerAuthenticationUrl = null;
            TransactionId = null;
            TransactionCreateDateTimestamp = default(DateTime);         
        }

        internal AuthenticationResponse(Exception e, string xml = null) : base(e, xml)
        {
            IssuerAuthenticationUrl = null;
            TransactionId = null;
            TransactionCreateDateTimestamp = default(DateTime);
        }

        internal static AuthenticationResponse Parse(string xml)
        {
            try
            {
                if (xml.Contains("AcquirerTrxRes"))
                {
                    var dirRes = AcquirerTrxRes.Deserialize(xml);
                    return new AuthenticationResponse(dirRes, xml);
                }

                var errRes = AcquirerErrorRes.Deserialize(xml);
                return new AuthenticationResponse(errRes, xml);
            }
            catch (Exception e)
            {
                return new AuthenticationResponse(e, xml);                
            }
        }
    }
}
