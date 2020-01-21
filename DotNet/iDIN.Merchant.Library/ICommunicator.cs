using System.Threading.Tasks;

namespace BankId.Merchant.Library
{
    /// <summary>
    /// ICommunicator interface, implemented by <see cref="Communicator"/>.
    /// </summary>
    public interface ICommunicator
    {
        /// <summary>
        ///     Sends a directory request
        /// </summary>
        /// <returns>
        ///     A DirectoryResponse object which contains the response from the server (a list of Issuers), or error information when an error occurs
        /// </returns>
        DirectoryResponse GetDirectory();

        /// <summary>
        ///     Sends a transaction request
        /// </summary>
        /// <param name="authenticationRequest">An AuthenticationRequest object</param>
        /// <returns>
        ///     An AuthenticationResponse object which contains the response from the server (transaction id, issuer authentication URL), or error information when an error occurs
        /// </returns>
        AuthenticationResponse NewAuthenticationRequest(AuthenticationRequest authenticationRequest);

        /// <summary>
        ///     Sends a transaction status request to the URL specified in Configuration.AcquirerUrl_TransactionReq
        /// </summary>
        /// <param name="statusRequest">A StatusRequest object</param>
        /// <returns>
        ///     A StatusResponse object which contains the response from the server (transaction id, status message), or error information when an error occurs
        /// </returns>
        StatusResponse GetResponse(StatusRequest statusRequest);

        /// <summary>
        ///     Async version of GetDirectory method
        /// </summary>
        /// <returns>
        ///     A DirectoryResponse object which contains the response from the server (a list of Issuers), or error information when an error occurs
        /// </returns>
        Task<DirectoryResponse> GetDirectoryAsync();

        /// <summary>
        ///    Async version of NewAuthenticationRequest
        /// </summary>
        /// <param name="authenticationRequest">An AuthenticationRequest object</param>
        /// <returns>
        ///     An AuthenticationResponse object which contains the response from the server (transaction id, issuer authentication URL), or error information when an error occurs
        /// </returns>
        Task<AuthenticationResponse> NewAuthenticationRequestAsync(AuthenticationRequest authenticationRequest);

        /// <summary>
        ///     Async version of GetResponse
        /// </summary>
        /// <param name="statusRequest">A StatusRequest object</param>
        /// <returns>
        ///     A StatusResponse object which contains the response from the server (transaction id, status message), or error information when an error occurs
        /// </returns>
        Task<StatusResponse> GetResponseAsync(StatusRequest statusRequest);
    }
}
