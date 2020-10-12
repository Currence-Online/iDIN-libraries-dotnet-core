using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BankId.Merchant.Library.Tests.Utilities
{
    public class TestMessenger : IMessenger
    {
        public string Request { get; private set; }
        public string Response { get; set; }

        public TestMessenger(string response)
        {
            Request = "";
            Response = response;
        }

        public string SendMessage(string message, Uri url)
        {
            Request = message;
            return Response;
        }

        public Task<string> SendMessageAsync(string message, Uri url)
        {
            Request = message;
            return Task.FromResult(Response);
        }
    }
}