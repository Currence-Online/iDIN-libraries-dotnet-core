using System;
using BankId.Merchant.Library.Xml.Schemas.iDx;
using BankId.Merchant.Library.Xml.Schemas.Saml.Protocol;

namespace BankId.Merchant.Library
{
    /// <summary>
    /// Describes an error response
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Unique identification of the error occurring within the iDx transaction
        /// </summary>
        public string ErrorCode { get; private set; }

        /// <summary>
        /// Descriptive text accompanying Error.errorCode
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Details of the error
        /// </summary>
        public string ErrorDetails { get; private set; }

        /// <summary>
        /// Suggestions aimed at resolving the problem
        /// </summary>
        public string SuggestedAction { get; private set; }

        /// <summary>
        /// A (standardised) message that the merchant should show to the consumer
        /// </summary>
        public string ConsumerMessage { get; private set; }

        /// <summary>
        /// Details of the SAML status.
        /// </summary>
        public SamlStatus AdditionalInformation { get; private set; }

        /// <summary>
        /// Details of the exception occurred during the processing of request/response.
        /// </summary>
        public Exception Exception { get; private set; }


        internal ErrorResponse(AcquirerErrorRes errRes)
        {           
            ErrorCode = errRes.Error.errorCode;
            ErrorMessage = errRes.Error.errorMessage;
            ErrorDetails = errRes.Error.errorDetail;
            SuggestedAction = errRes.Error.suggestedAction;
            ConsumerMessage = errRes.Error.consumerMessage;
            Exception = null;


            var samlXml = errRes.Error.container?.Any[0].OuterXml;
            if (string.IsNullOrWhiteSpace(samlXml)) return;

            var responseType = ResponseType.Deserialize(samlXml);
            if (responseType.Status != null)
            {
                AdditionalInformation = new SamlStatus(responseType.Status.StatusMessage, responseType.Status?.StatusCode?.Value, responseType.Status?.StatusCode?.StatusCode?.Value);                 
            }
        }

        internal ErrorResponse(Exception e)
        {
            Exception = e;
            ErrorMessage = e.Message;
            ErrorCode = ErrorDetails = SuggestedAction = ConsumerMessage = string.Empty;
            AdditionalInformation = null;
        }

        internal ErrorResponse(SamlStatus samlStatus)
        {           
            Exception = null;
            ErrorMessage = "SAML specific error.";
            ErrorCode = ErrorDetails = SuggestedAction = ConsumerMessage = string.Empty;
            AdditionalInformation = samlStatus;

        }

        public override string ToString()
        {
            return $"{nameof(ErrorCode)}: {ErrorCode}, {nameof(ErrorMessage)}: {ErrorMessage}, {nameof(ErrorDetails)}: {ErrorDetails}, {nameof(SuggestedAction)}: {SuggestedAction}, {nameof(ConsumerMessage)}: {ConsumerMessage}, {nameof(AdditionalInformation)}: {AdditionalInformation}, {nameof(Exception)}: {Exception}";
        }
    }
}
