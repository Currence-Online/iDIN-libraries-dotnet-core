namespace BankId.Merchant.Library.Security
{
    /// <summary>
    /// IXmlSecurity interface: utility methods on XML files
    /// </summary>
    public interface IXmlSecurity
    {
        /// <summary>
        /// Sign the xml passed as a parameter using the certificate in Configuration.SigningCertificate
        /// </summary>
        string AddSignature(string xml);
        
        /// <summary>
        /// Verify the signature of an xml
        /// </summary>
        bool VerifySignature(string xml);

        /// <summary>
        /// Verify that the xml is correct according to the XSDs
        /// </summary>
        bool VerifySchema(string xml);
    }
}
