using System.Diagnostics;
using System.IO;

namespace BankId.Merchant.Library.SampleWebsite
{
    public class CustomTraceListener : TraceListener
    {
        private readonly string _webRootPath;

        public CustomTraceListener(string webRootPath) : base()
        {
            _webRootPath = webRootPath;
        }
        public override void Write(string message)
        {
            lock(this)
            {
                using (var sw = new StreamWriter(Path.Combine(_webRootPath, "logs/info-log.txt"), true))
                {
                    sw.WriteLine(message);
                }
            }
        }

        public override void WriteLine(string message)
        {
            lock (this)
            {
                using (var sw = new StreamWriter(Path.Combine(_webRootPath, "logs/info-log.txt"), true))
                {
                    sw.WriteLine(message);
                }
            }
        }
    }
}