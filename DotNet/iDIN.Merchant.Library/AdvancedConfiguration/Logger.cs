using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace BankId.Merchant.Library.AdvancedConfiguration
{
    internal enum LogLevel
    {
        Debug,
        None
    };

    /// <summary>
    /// The default logger used by the library
    /// </summary>
    internal class Logger : ILogger
    {
        private readonly IConfiguration _configuration;

        public Logger(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Logs a trace message to System.Debug.Trace.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Log(string message = null, params object[] args)
        {
            Write(LogLevel.Debug, new StackTrace().GetFrame(1).GetMethod(), message, args);
        }

        /// <summary>
        /// Logs a request/response xml message to the directory specified in the configuration.
        /// </summary>
        public void LogXmlMessage(string content)
        {
            if (!_configuration.ServiceLogsEnabled)
            {
                return;
            }
            var xmldoc = new XmlDocument();
            xmldoc.LoadXml(content);
            var now = DateTime.Now;

            var fileName = _configuration.ServiceLogsPattern;
            
            fileName = fileName.Replace("%Y", now.ToString("yyyy"));
            fileName = fileName.Replace("%M", now.ToString("MM"));
            fileName = fileName.Replace("%D", now.ToString("dd"));
            fileName = fileName.Replace("%h", now.ToString("HH"));
            fileName = fileName.Replace("%m", now.ToString("mm"));
            fileName = fileName.Replace("%s", now.ToString("ss"));
            fileName = fileName.Replace("%f", now.ToString("fff"));
            fileName = fileName.Replace("%a", Sanitize(xmldoc.DocumentElement.LocalName));
            fileName = Path.Combine(_configuration.ServiceLogsLocation, fileName);

            Log("writing to: " + fileName);
            var file = new FileInfo(fileName);

            Log("creating: " + file.DirectoryName);
            Directory.CreateDirectory(file.DirectoryName);

            File.WriteAllText(file.FullName, content);
        }

        
        private static readonly Regex Sanitizer = new Regex("[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]");
        private static string Sanitize(string fileName)
        {
            return Sanitizer.Replace(fileName, "");
        }

        private void Write(LogLevel level, MethodBase mi, string message, params object[] args)
        {
            var str = string.Format(message, args);
            var method = mi.DeclaringType.Name + "." + mi.Name + "()";
            str = string.Format("{0} [{1}] [{2}] {3} - {4}", DateTime.Now.ToString("O"), Thread.CurrentThread.ManagedThreadId.ToString().PadLeft(4), level.ToString().PadLeft(5), method, str);

            Trace.WriteLine(str);
        }
    }
}
