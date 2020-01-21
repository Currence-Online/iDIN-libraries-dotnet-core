using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BankId.Merchant.Library
{
    internal class Messenger : IMessenger
    {
        private readonly ILogger _logger;

        public Messenger(IConfiguration configuration)
        {
            _logger = configuration.GetLogger();
        }

        public async Task<string> SendMessageAsync(string message, Uri url)
        {
            using (var httpClient = new HttpClient())
            {
                var result = await httpClient.PostAsync(url, new StringContent(message, System.Text.Encoding.UTF8, "text/xml"));

                _logger.Log("result status: {0}", result.StatusCode);
                if (!result.IsSuccessStatusCode)
                {
                    _logger.Log("http request failed: {0}", result.StatusCode);
                    throw new CommunicatorException("http request failed, code=" + result.StatusCode);
                }

                var content = await result.Content.ReadAsStringAsync();
                return content;
            }
        }
    }
}