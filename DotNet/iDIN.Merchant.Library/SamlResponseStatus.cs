namespace BankId.Merchant.Library
{
    /// <summary>
    /// Enumeration of possible Saml Response Status
    /// </summary>
    public enum SamlResponseStatus
    {
        /// <summary>
        /// SAML: The request succeeded
        /// </summary>
        Success = 0,
        /// <summary>
        /// SAML: The request could not be performed due to an error on the part of the requester.
        /// </summary>
        Requester,
        /// <summary>
        /// SAML: The request could not be performed due to an error on the part of the SAML responder or SAML authority.
        /// </summary>
        Responder,
        /// <summary>
        /// SAML: The responding provider was unable to successfully authenticate the principal.
        /// </summary>
        AuthenticationFailed
    }
}