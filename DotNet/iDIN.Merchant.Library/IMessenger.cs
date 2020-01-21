using System;
using System.Threading.Tasks;

namespace BankId.Merchant.Library
{
    /// <summary>
    /// IMessenger interface, implemented by <see cref="Messenger"/>.
    /// </summary>
    public interface IMessenger
    {
        /// <summary>
        /// Sends the specified message to the given url. Async version of SendMessage/
        /// </summary>
        /// <param name="message">Message to be sent</param>
        /// <param name="url">Url to send the message</param>
        /// <returns></returns>
        Task<string> SendMessageAsync(string message, Uri url);
    }
}
