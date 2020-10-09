using System.IO;
using BankId.Server.Web.Models;

namespace BankId.Merchant.Library.Tests.Utilities
{
    public class TestMessage
    {
        private static readonly XmlProcessor processor = new XmlProcessor(Directories.Certificates);

        public static string Get(string name)
        {
            var message =  File.ReadAllText(Path.Combine(Directories.Messages, name + ".xml"));
            return processor.AddSignature(message);
        }
    }
}