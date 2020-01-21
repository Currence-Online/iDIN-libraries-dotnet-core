namespace BankId.Merchant.Library.MessageBuilders
{
    /// <summary>
    /// IIDxMessageBuilder interface, implemented by <see cref="IDxMessageBuilder"/>.
    /// </summary>
    internal interface IIDxMessageBuilder
    {
        string GetDirectoryRequest();
        string GetStatusRequest(StatusRequest statusRequest);
        string GetTransactionRequest(AuthenticationRequest authenticationRequest, string containedData);
    }
}