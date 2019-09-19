using System;
using System.Linq;
using BankId.Merchant.Library.Xml.Schemas.iDx;

namespace BankId.Merchant.Library
{
    /// <summary>
    /// Describes a directory response
    /// </summary>
    public class DirectoryResponse : BaseResponse
    {
        /// <summary>
        /// DateTime set to when this directory was last updated
        /// </summary>
        public DateTime DirectoryDateTimestamp { get; private set; }

        /// <summary>
        /// List of available Issuers
        /// </summary>
        public Issuer[] Issuers { get; private set; }

        private DirectoryResponse(DirectoryRes dirRes, string xml) : base(xml)
        {
            DirectoryDateTimestamp = dirRes.Directory.directoryDateTimestamp;

            Issuers = (
                from country in dirRes.Directory.Country
                from issuer in country.Issuer
                select new Issuer
                {
                    Country = country.countryNames,
                    Id = issuer.issuerID,
                    Name = issuer.issuerName
                }).ToArray();
        }

        private DirectoryResponse(AcquirerErrorRes errRes, string xml)
            : base(errRes, xml)
        {
            
            DirectoryDateTimestamp = default(DateTime);
            Issuers = null;
           
        }

        internal DirectoryResponse(Exception ex, string xml = null)
            : base(ex, xml)
        {
            DirectoryDateTimestamp = default(DateTime);
            Issuers = null;
        }

        internal static DirectoryResponse Parse(string xml)
        {
            try
            {
                if (xml.Contains("DirectoryRes"))
                {
                    var dirRes = DirectoryRes.Deserialize(xml);
                    return new DirectoryResponse(dirRes, xml);
                }

                var errRes = AcquirerErrorRes.Deserialize(xml);
                return new DirectoryResponse(errRes, xml);
            }
            catch (Exception e)
            {
                return new DirectoryResponse(e, xml);
            }
        }
    }
}
