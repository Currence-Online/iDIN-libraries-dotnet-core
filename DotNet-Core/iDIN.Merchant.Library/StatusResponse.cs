using System;
using BankId.Merchant.Library.Xml.Schemas.iDx;

namespace BankId.Merchant.Library
{
    /// <summary>
    /// Represents a status response
    /// </summary>
    public class StatusResponse : BaseResponse
    {
#pragma warning disable 1591
        /// <summary>
        /// Status Open
        /// </summary>
        public static readonly string Open = "Open";
        /// <summary>
        /// Status Pending
        /// </summary>
        public static readonly string Pending = "Pending";
        /// <summary>
        /// Status Success
        /// </summary>
        public static readonly string Success = "Success";
        /// <summary>
        /// Status Failure
        /// </summary>
        public static readonly string Failure = "Failure";
        /// <summary>
        /// Status Expired
        /// </summary>
        public static readonly string Expired = "Expired";
        /// <summary>
        /// Status Cancelled
        /// </summary>
        public static readonly string Cancelled = "Cancelled";
#pragma warning restore 1591

        /// <summary>
        /// The transaction ID
        /// </summary>
        public string TransactionId { get; private set; }

        /// <summary>
        /// Possible values: Open, Pending, Success, Failure, Expired, Cancelled
        /// </summary>
        public string Status { get; }

        /// <summary>
        /// DateTime when the status was created, or null if no such date available
        /// </summary>
        public DateTime? StatusDateTimestamp { get; private set; }

        /// <summary>
        /// The SAML report returned in the status response
        /// </summary>
        public SamlResponse SamlResponse { get; }

        private StatusResponse(AcquirerStatusRes statusRes, string xml, IConfiguration configuration)
            : base(xml)
        {
            TransactionId = statusRes.Transaction.transactionID;
            StatusDateTimestamp =
                statusRes.Transaction.statusDateTimestampSpecified ? statusRes.Transaction.statusDateTimestamp : (DateTime?)null;
            Status = statusRes.Transaction.status;

            if (Status != Success) return;

            if (statusRes.Transaction.container == null)
                throw new Exception("No SAML message present for the transaction with status 'Success'.");

            var samlXml = statusRes.Transaction.container.Any[0].OuterXml;

            if (string.IsNullOrWhiteSpace(samlXml)) return;

            SamlResponse = SamlResponse.Parse(samlXml, configuration);

            // check for invalid SAML status
            if (SamlResponse.Status?.StatusCodeFirstLevel == SamlStatus.Success) return;

            Error = new ErrorResponse(SamlResponse.Status);
            IsError = true;
        }

        private StatusResponse(AcquirerErrorRes errRes, string xml)
            : base(errRes, xml)
        {
            TransactionId = null;
            Status = null;
            StatusDateTimestamp = null;
        }

        internal StatusResponse(Exception ex, string xml = null)
            : base(ex, xml)
        {
            TransactionId = null;
            Status = null;
            StatusDateTimestamp = null;
        }

        internal static StatusResponse Parse(string xml, IConfiguration configuration)
        {
            try
            {
                if (xml.Contains("AcquirerStatusRes"))
                {
                    var statusRes = AcquirerStatusRes.Deserialize(xml);
                    return new StatusResponse(statusRes, xml, configuration);
                }

                var errRes = AcquirerErrorRes.Deserialize(xml);
                return new StatusResponse(errRes, xml);
            }
            catch (Exception e)
            {
                return new StatusResponse(e, xml);
            }
        }
    }
}
