using System.Diagnostics;
using System.IO;

namespace BankId.Merchant.Library.SampleWebsite
{
    public class CustomTraceListener : TraceListener
    {
        private readonly string _webPath;

        public CustomTraceListener(string webPath) : base()
        {
            _webPath = webPath;
        }
        public override void Write(string message)
        {
            lock(this)
            {
                string logFilePath = Path.Combine(_webPath, "info-log.txt");
                using (StreamWriter sw = (File.Exists(logFilePath)) ? File.AppendText(logFilePath) : File.CreateText(logFilePath))
                {
                    sw.WriteLine(message);
                }
            }
        }

        public override void WriteLine(string message)
        {
            lock (this)
            {
                string logFilePath = Path.Combine(_webPath, "info-log.txt");
                using (StreamWriter sw = (File.Exists(logFilePath)) ? File.AppendText(logFilePath) : File.CreateText(logFilePath))
                {
                    sw.WriteLine(message);
                }
            }
        }
    }
}