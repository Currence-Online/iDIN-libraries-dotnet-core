using System;
using BankId.Merchant.Library.MessageBuilders;
using BankId.Merchant.Library.Security;

namespace BankId.Merchant.Library
{
    /// <summary>
    /// Class responsible for communicating with the services.
    /// </summary>
    public sealed class Communicator : ICommunicator
    {
        /// <summary>
        /// ILogger instance, to be used for logging messages.
        /// </summary>
        private readonly ILogger _logger;
        /// <summary>
        /// IMessebger instance, to be used for sending messages to external URIs
        /// </summary>
        private readonly IMessenger _messenger;

        /// <summary>
        /// XmlSecurity instance, used to process XMLs (signing, verifying, validating signature)
        /// </summary>
        private readonly IXmlSecurity _xmlSecurity;

        /// <summary>
        /// IConfiguration instance used to hold all configuration keys
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Creates a new Communicator instance
        /// </summary>
        public Communicator()
        {
            var config = Configuration.Instance;

            if (config == null)
                throw new CommunicatorException("The configuration has not been initialized.");

            _configuration = config;

            _logger = _configuration.GetLogger();
            _messenger = _configuration.GetMessenger();

            _xmlSecurity = new XmlSecurity(_configuration);

            
            _logger.Log("communicator initialized");
        }


        /// <summary>
        /// Creates a new Communicator instance by specifing a custom configuration instance
        /// </summary>
        /// <param name="configuration"></param>
        public Communicator(IConfiguration configuration)
        {
            if (configuration == null)
                throw new CommunicatorException("The configuration has not been initialized.");

            _configuration = configuration;

            _logger = _configuration.GetLogger();
            _messenger = _configuration.GetMessenger();

            _xmlSecurity = new XmlSecurity(_configuration);


            _logger.Log("communicator initialized with custom configuration");
        }

        #region private methods

        /// <summary>
        /// Sign a message using Configuration.Instance.SigningCertificate
        /// </summary>
        private string Sign(string xml)
        {
            return _xmlSecurity.AddSignature(xml);
        }

        /// <summary>
        /// Verify an incoming message's signature using Configuration.Instance.AcquirerCertificate
        /// </summary>
        private bool VerifySignature(string xml)
        {
            return _xmlSecurity.VerifySignature(xml);
        }

        /// <summary>
        /// Verify that a message is correct according to the XML schemas
        /// </summary>
        private bool VerifySchema(string xml)
        {
            return _xmlSecurity.VerifySchema(xml);
        }

        /// <summary>
        /// Perform the http(s) request and return the result
        /// </summary>
        private string PerformRequest(string xml, Uri url)
        {
            _logger.Log("sending request to {0}", url);

            try
            {
                VerifySchema(xml);
            }
            catch (Exception e)
            {
                _logger.Log("request xml schema is not valid: {0}", e.Message);
                throw new CommunicatorException("Request XML schema is not valid.", e);
            }

            _logger.LogXmlMessage(xml);

            var content = _messenger.SendMessage(xml, url);

            _logger.LogXmlMessage(content);

            try
            {
                VerifySchema(content);
                _logger.Log("response xml schema is valid");
            }
            catch (Exception e)
            {
                _logger.Log("response xml schema is not valid: {0}", e.Message);
                throw new CommunicatorException("Response XML schema is not valid", e);
            }

            bool signatureIsValid = VerifySignature(content);
            _logger.Log("signature is valid: {0}", signatureIsValid);
            if (!signatureIsValid)
            {
                _logger.Log("request xml signature is not valid");
                throw new CommunicatorException("Response XML signature is not valid");
            }

            return content;
        }

        #endregion 

        /// <summary>
        ///     Sends a directory request to the URL specified in Configuration.AcquirerUrl_DirectoryReq
        /// </summary>
        /// <returns>
        ///     A DirectoryResponse object which contains the response from the server (a list of Issuers), or error information when an error occurs
        /// </returns>
        public DirectoryResponse GetDirectory()
        {
            try
            {
                _logger.Log("sending new directory request");

                _logger.Log("building idx message");
                var directoryreq = new IDxMessageBuilder(_configuration).GetDirectoryRequest();

                _logger.Log("signing message");
                var xml = Sign(directoryreq);

                var content = PerformRequest(xml, _configuration.AcquirerDirectoryUrl);

                return DirectoryResponse.Parse(content);
            }
            catch (Exception e)
            {
                _logger.Log(e.ToString());
                return new DirectoryResponse(e);
            }
        }

        /// <summary>
        ///     Sends a new authentication request to the URL specified in Configuration.AcquirerUrl_TransactionReq
        /// </summary>
        /// <param name="authenticationRequest">An AuthenticationRequest object</param>
        /// <returns>
        ///     An AuthenticationResponse object which contains the response from the server (transaction id, issuer authentication URL), or error information when an error occurs
        /// </returns>
        public AuthenticationResponse NewAuthenticationRequest(AuthenticationRequest authenticationRequest)
        {
            try
            {
                _logger.Log("sending new authentication request");

                _logger.Log("building request");
                var document = new BankIdMessageBuilder(_configuration).GetNewTransaction(authenticationRequest);

                _logger.Log("building idx message");
                var acquirertrxreq =
                    new IDxMessageBuilder(_configuration).GetTransactionRequest(authenticationRequest, document);

                _logger.Log("signing message");
                var xml = Sign(acquirertrxreq);

                var content = PerformRequest(xml, _configuration.AcquirerTransactionUrl);

                return AuthenticationResponse.Parse(content);
            }
            catch (Exception e)
            {
                _logger.Log(e.ToString());
                return new AuthenticationResponse(e);
            }
        }

        /// <summary>
        ///     Sends a transaction status request to the URL specified in Configuration.AcquirerUrl_TransactionReq
        /// </summary>
        /// <param name="statusRequest">A StatusRequest object</param>
        /// <returns>
        ///     A StatusResponse object which contains the response from the server (transaction id, status message), or error information when an error occurs
        /// </returns>
        public StatusResponse GetResponse(StatusRequest statusRequest)
        {
            try
            {
                _logger.Log("sending new status request");

                _logger.Log("building idx message");
                var acquirerstsreq =
                    new IDxMessageBuilder(_configuration).GetStatusRequest(statusRequest);

                _logger.Log("signing message");
                var xml = Sign(acquirerstsreq);

                var content = PerformRequest(xml, _configuration.AcquirerStatusUrl);

                return StatusResponse.Parse(content, _configuration);
            }
            catch (Exception e)
            {
                _logger.Log(e.ToString());
                return new StatusResponse(e);
            }
        }
    }
    
}
