using System;
using System.Xml;
using BankId.Merchant.Library.SampleWebsite.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankId.Merchant.Library.SampleWebsite.Controllers
{
    public class BankIdController : Controller
    {
        private IConfiguration _config;

        public BankIdController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View("Directory");
        }

        #region Directory
        [HttpGet]
        public ActionResult Directory()
        {
            var model = new DirectoryModel
            {
                DirectoryUrl = _config.AcquirerDirectoryUrl.AbsoluteUri,
                MerchantId = _config.MerchantId,
                ReturnUrl = _config.MerchantReturnUrl,
                SubId = _config.MerchantSubId
            };

            return View(model);
        }


        [HttpPost]
        public ActionResult Directory(DirectoryModel model)
        {
            _config.AcquirerDirectoryUrl = new Uri(model.DirectoryUrl);
            _config.MerchantId = model.MerchantId;
            _config.MerchantReturnUrl = model.ReturnUrl;
            _config.MerchantSubId = model.SubId;

            try
            {
                var communicator = new Communicator();
                model.Source = communicator.GetDirectory();
            }
            catch (Exception ex)
            {
                model.CustomError = ex.Message;
            }

            return View("DirectoryResponse", model);
        }
        #endregion

        #region AuthenticationRequest
        [HttpGet]
        public ActionResult AuthenticationRequest()
        {
            var model = new TransactionModel
            {
                IssuerID = "INGBNL2A",
                ExpirationPeriod = "PT5M",
                EntranceCode = "entranceCode",
                MerchantReference = "merchantReference",
                RequestedServiceId = "21952",
                AcquirerTransactionURL = _config.AcquirerTransactionUrl.AbsoluteUri,
                LOA = "nl:bvn:bankid:1.0:loa3",
                MerchantId = _config.MerchantId,
                ReturnUrl = _config.MerchantReturnUrl,
                SubId = _config.MerchantSubId
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult AuthenticationRequest(TransactionModel model)
        {
            _config.AcquirerTransactionUrl = new Uri(model.AcquirerTransactionURL);
            _config.MerchantId = model.MerchantId;
            _config.MerchantReturnUrl = model.ReturnUrl;
            _config.MerchantSubId = model.SubId;

            try
            {
                //throw error if the AssuranceLevel is not appropriate
                var assuranceLevel = model.LOA == "nl:bvn:bankid:1.0:loa3" ? AssuranceLevel.Loa3 : (AssuranceLevel)0;

                var communicator = new Communicator();

                var serviceid = (ServiceIds)Enum.Parse(typeof(ServiceIds), model.RequestedServiceId);
                TimeSpan? time = null;

                if (!string.IsNullOrEmpty(model.ExpirationPeriod))
                    time = XmlConvert.ToTimeSpan(model.ExpirationPeriod);

                var transactionRequest = new AuthenticationRequest(model.EntranceCode, serviceid, model.IssuerID,
                    model.MerchantReference, assuranceLevel, time, model.Language ?? "nl", model.DocumentId);

                model.Source = communicator.NewAuthenticationRequest(transactionRequest);
            }
            catch (Exception ex)
            {
                model.CustomError = ex.Message;
            }

            return View("AuthenticationResponse", model);
        }
        #endregion


        #region GetResponse
        [HttpGet]
        public ActionResult GetResponse()
        {
            var model = new StatusModel
            {
                StatusUrl = _config.AcquirerStatusUrl.AbsoluteUri,
                TransactionId = "1234567890123456",
                MerchantId = _config.MerchantId,
                ReturnUrl = _config.MerchantReturnUrl,
                SubId = _config.MerchantSubId
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult GetResponse(StatusModel model)
        {
            _config.MerchantId = model.MerchantId;
            _config.MerchantReturnUrl = model.ReturnUrl;
            _config.MerchantSubId = model.SubId;
            _config.AcquirerStatusUrl = new Uri(model.StatusUrl);

            try
            {
                var communicator = new Communicator();

                var statusRequest = new StatusRequest(model.TransactionId);
                model.Source = communicator.GetResponse(statusRequest);
            }
            catch (Exception ex)
            {
                model.CustomError = ex.Message;
            }
            return View("StatusResponse", model);
        }
        #endregion
    }
}