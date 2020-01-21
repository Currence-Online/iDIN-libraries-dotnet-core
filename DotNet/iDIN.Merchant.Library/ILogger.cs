namespace BankId.Merchant.Library
{
    /// <summary>
    /// ILogger interface: defines methods for logging messages and debug output
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a trace message.
        /// </summary>
        void Log(string message = null, params object[] args);
        
        /// <summary>
        /// Logs a request/response xml message.
        /// </summary>
        void LogXmlMessage(string content);
    }
}
