namespace BankId.Merchant.Library
{
    /// <summary>
    /// An Issuer contained in a directory response
    /// </summary>
    public class Issuer
    {
        /// <summary>
        /// Country name
        /// </summary>
        public string Country { get; internal set; }

        /// <summary>
        /// BIC
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// Bank name
        /// </summary>
        public string Name { get; internal set; }
    }
}