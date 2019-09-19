using System;

namespace BankId.Merchant.Library
{
    /// <summary>
    /// Base exception used by the Communicator class
    /// </summary>
    [Serializable]
    public class CommunicatorException : Exception
    {
        /// <summary>
        /// Constructor without parameters
        /// </summary>
        public CommunicatorException()
        {
        }

        /// <summary>
        /// Constructor that sets exception message
        /// </summary>
        /// <param name="message">The message to set</param>
        public CommunicatorException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor that sets exception message and the InnerException
        /// </summary>
        /// <param name="message">The message to set</param>
        /// <param name="inner">The inner exception</param>
        public CommunicatorException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
