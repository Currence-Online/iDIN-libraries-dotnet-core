namespace BankId.Merchant.Library
{
    /// <summary>
    /// Class responsible for handling Saml Statuses
    /// </summary>
    public sealed class SamlStatus
    {
#pragma warning disable 1591
        /// <summary>
        /// Status Success
        /// </summary>
        public static readonly string Success = "urn:oasis:names:tc:SAML:2.0:status:Success";
        /// <summary>
        /// Status Requester
        /// </summary>
        public static readonly string Requester = "urn:oasis:names:tc:SAML:2.0:status:Requester";
        /// <summary>
        /// Status RequestDenied
        /// </summary>
        public static readonly string RequestDenied = "urn:oasis:names:tc:SAML:2.0:status:RequestDenied";
        /// <summary>
        /// Status RequestUnsupported
        /// </summary>
        public static readonly string RequestUnsupported = "urn:oasis:names:tc:SAML:2.0:status:RequestUnsupported";
        /// <summary>
        /// Status InvalidAttrNameOrValue
        /// </summary>
        public static readonly string InvalidAttrNameOrValue = "urn:oasis:names:tc:SAML:2.0:status:InvalidAttrNameOrValue";
        /// <summary>
        /// Status MismatchWithIDx
        /// </summary>
        public static readonly string MismatchWithIDx = "urn:nl:bvn:bankid:1.0:status:MismatchWithIDx";
#pragma warning restore 1591

        /// <summary>
        /// The status message
        /// </summary>
        public string StatusMessage { get; private set; }
        /// <summary>
        /// The status code first level
        /// </summary>
        public string StatusCodeFirstLevel { get; private set; }
        /// <summary>
        /// The status code second level
        /// </summary>
        public string StatusCodeSecondLevel { get; private set; }

        /// <summary>
        /// Constructor used to create Saml Status objects
        /// </summary>
        /// <param name="statusMessage">The status message</param>
        /// <param name="statusCodeFirstLevel">The status code first level</param>
        /// <param name="statusCodeSecondLevel">The status code second level</param>
        internal SamlStatus(string statusMessage, string statusCodeFirstLevel, string statusCodeSecondLevel)
        {
            StatusMessage = statusMessage;
            StatusCodeFirstLevel = statusCodeFirstLevel;
            StatusCodeSecondLevel = statusCodeSecondLevel;
        }
    }
}
