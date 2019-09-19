using System;

namespace BankId.Merchant.Library.SampleWebsite.Models
{
    public class AdvancedOptions
    {
        public string MerchantId { get; set; }
        public uint SubId { get; set; }
        public Uri ReturnUrl { get; set; }

        public string CustomError { get; set; }
    }
}