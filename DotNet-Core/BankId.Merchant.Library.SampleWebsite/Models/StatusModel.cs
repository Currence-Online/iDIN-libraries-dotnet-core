namespace BankId.Merchant.Library.SampleWebsite.Models
{
    public class StatusModel : AdvancedOptions
    {
        public string StatusUrl { get; set; }
        public string TransactionId { get; set; }

        public StatusResponse Source { get; set; }
    }
}