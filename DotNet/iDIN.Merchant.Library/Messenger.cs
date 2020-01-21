using System;
using System.Net.Http;

namespace BankId.Merchant.Library
{
    /// <summary>
    /// IMessenger interface, implemented by <see cref="Messenger"/>.
    /// </summary>
    public interface IMessenger
    {
        /// <summary>
        /// Sends the specified message to the given url
        /// </summary>
        /// <param name="message">Message to be sent</param>
        /// <param name="url">Url to send the message</param>
        /// <returns></returns>
        string SendMessage(string message, Uri url);
    }

    internal class Messenger : IMessenger
    {
        private readonly ILogger _logger;

        public Messenger(IConfiguration configuration)
        {
            _logger = configuration.GetLogger();
        }

        public string SendMessage(string message, Uri url)
        {
            using (var httpClient = new HttpClient())
            {
                //httpClient.DefaultRequestHeaders.Add("Content-Type", "text/xml; charset='utf-8'");
                var task = httpClient.PostAsync(url, new StringContent(message, System.Text.Encoding.UTF8, "text/xml"));
                
                var result = task.Result;
                _logger.Log("result status: {0}", result.StatusCode);
                if (!result.IsSuccessStatusCode)
                {
                    _logger.Log("http request failed: {0}", result.StatusCode);
                    throw new CommunicatorException("http request failed, code=" + result.StatusCode);
                }

                var content = result.Content.ReadAsStringAsync().Result;
                return content;
            }
        }
    }
}