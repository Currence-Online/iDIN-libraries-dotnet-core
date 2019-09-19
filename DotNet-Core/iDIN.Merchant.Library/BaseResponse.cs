using System;
using BankId.Merchant.Library.Xml.Schemas.iDx;

namespace BankId.Merchant.Library
{
    public abstract class BaseResponse
    {
        /// <summary>
        /// true if an error occured, or false when no errors were encountered
        /// </summary>
        public bool IsError { get; protected set; }

        /// <summary>
        /// Object that holds the error if one occurs; when there are no errors, this is set to null
        /// </summary>
        public ErrorResponse Error { get; protected set; }

        /// <summary>
        /// The response XML as received
        /// </summary>
        public string RawMessage { get; private set; }

        public BaseResponse(string xml)
        {
            Error = null;
            IsError = false;
            RawMessage = xml;
        }

        protected BaseResponse(AcquirerErrorRes errRes, string xml)
        {
            Error = new ErrorResponse(errRes);
            IsError = true;
            RawMessage = xml;
        }

        protected BaseResponse(Exception ex)
        {
            Error = new ErrorResponse(ex);
            IsError = true;
        }

        protected BaseResponse(Exception ex, string xml) : this(ex)
        {
            RawMessage = xml;
        }
    }
}