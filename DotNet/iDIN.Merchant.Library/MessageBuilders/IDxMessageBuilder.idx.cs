using System;
using System.Globalization;
using System.Xml;
using BankId.Merchant.Library.Helpers;
using BankId.Merchant.Library.Xml.Schemas.iDx;

namespace BankId.Merchant.Library.MessageBuilders
{
    internal class IDxMessageBuilder : IIDxMessageBuilder
    {
        private readonly IConfiguration _configuration;
        
        private readonly string[] _dateTimeElementNames = {"directoryDateTimestamp", "createDateTimestamp", "transactionCreateDateTimestamp", "statusDateTimestamp"};

        public IDxMessageBuilder(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private void VerifyExpirationPeriod(TimeSpan? expirationPeriod)
        {
            if (expirationPeriod != null && expirationPeriod.Value > TimeSpan.FromDays(7))
            {
                throw new CommunicatorException("ExpirationPeriod should be less than 7 days");
            }
        }

        public string GetDirectoryRequest()
        {
            var directoryRequest = new DirectoryReq
            {
                createDateTimestamp = DateTimeHelper.Now(),
                version = "1.0.0",
                productID = Constants.IDxProductId,

                Merchant = new DirectoryReqMerchant
                {
                    merchantID = _configuration.MerchantId,
                    subID = _configuration.MerchantSubId.ToString(CultureInfo.InvariantCulture)
                }
            };

            return DateTimeHelper.ProcessDateTimes(directoryRequest.Serialize(), _dateTimeElementNames);
        }

        public string GetTransactionRequest(AuthenticationRequest authenticationRequest, string containedData)
        {
            if (authenticationRequest == null) throw new ArgumentNullException("authenticationRequest");
            if (containedData == null) throw new ArgumentNullException("containedData");

            VerifyExpirationPeriod(authenticationRequest.ExpirationPeriod);

            var containedDocument = new XmlDocument();
            containedDocument.LoadXml(containedData);

            var acquirerTrxReq = new AcquirerTrxReq
            {
                createDateTimestamp = authenticationRequest.CreateDateTimestamp,
                version = "1.0.0",
                productID = Constants.IDxProductId,

                Merchant = new AcquirerTrxReqMerchant
                {
                    merchantID = _configuration.MerchantId,
                    subID = _configuration.MerchantSubId.ToString(CultureInfo.InvariantCulture),
                    merchantReturnURL = _configuration.MerchantReturnUrl.AbsoluteUri
                },

                Issuer = new AcquirerTrxReqIssuer
                {
                    issuerID = authenticationRequest.IssuerId
                },

                Transaction = new AcquirerTrxReqTransaction
                {
                    entranceCode = authenticationRequest.EntranceCode,

                    expirationPeriod = authenticationRequest.ExpirationPeriod.HasValue ? XmlConvert.ToString(authenticationRequest.ExpirationPeriod.Value) : null,
                    language = authenticationRequest.Language,
                    container = new Transactioncontainer
                    {
                        Any = new [] { containedDocument.DocumentElement }
                    }
                }
            };            

            return DateTimeHelper.ProcessDateTimes(acquirerTrxReq.Serialize(), _dateTimeElementNames);
        }

        public string GetStatusRequest(StatusRequest statusRequest)
        {
            if (statusRequest == null) throw new ArgumentNullException("statusRequest");

            var acquirerStatusReq = new AcquirerStatusReq
            {
                createDateTimestamp = DateTimeHelper.Now(),
                productID = Constants.IDxProductId,
                version = "1.0.0",

                Merchant = new AcquirerStatusReqMerchant
                {
                    merchantID = _configuration.MerchantId,
                    subID = _configuration.MerchantSubId.ToString(CultureInfo.InvariantCulture)
                },

                Transaction = new AcquirerStatusReqTransaction
                {
                    transactionID = statusRequest.TransactionId
                }
            };

            return DateTimeHelper.ProcessDateTimes(acquirerStatusReq.Serialize(), _dateTimeElementNames);
        }
    }
}
