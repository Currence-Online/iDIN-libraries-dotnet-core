namespace BankId.Merchant.Library
{
    public class SamlAttributesEncryptionKey
    {
        public string AttributeName { get; set; }
        public byte[] AesKey { get; set; }
    }
}
