using System;
using Microsoft.Extensions.Configuration;

//using SharedKernel.Extensions;
//using SharedKernel.Interfaces;
//using SharedKernel.Interfaces.Providers;
//using SharedKernel.Providers;
//using SharedKernel.Utils;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using QuickService.LoanRepayment.Core.Utils;
using QuickService.LoanRepayment.Core.Constants;
using QuickService.LoanRepayment.Core.DTO.APIRequest;
using System.Xml.Serialization;
using System.IO;
using QuickService.LoanRepayment.Core.DTO.APIResponse;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using QuickService.LoanRepayment.Core.DTO.Services;
using QuickService.LoanRepayment.Core.Interfaces;
using QuickService.LoanRepayment.Infrastructure.Common;
using QuickService.LoanRepayment.Infrastructure.Helpers.Interfaces;
using QuickService.LoanRepayment.Infrastructure.Services.RedboxServiceProxies.Interfaces;

namespace QuickService.LoanRepayment.Infrastructure.Redbox
{
    public class RedboxManager : IRedboxManager
    {
        //private readonly IHttpClientService _httpClientService;
        private readonly IConfiguration _config;

        //private readonly ILogger<RedboxManager> _logger;

        private readonly IRedboxRequestManagerProxy _redboxRequestManager;
        private readonly ISoapRequestHelper _soapRequestHelper;

        private readonly IAppLogger _logger;
        //private readonly IHttpContextUtil _httpContextUtil;
        //private readonly IRedboxUtility _redboxUtility;
        //private readonly IFileLogger _fileLogger;

        //public RedboxManager(IHttpClientService httpClientService, IConfiguration config,
        //    IHttpContextUtil httpContextUtil, IRedboxUtility redboxUtility, IFileLogger fileLogger)
        //{
        //    _httpClientService = httpClientService;
        //    _config = config;
        //    _httpContextUtil = httpContextUtil;
        //    _redboxUtility = redboxUtility;
        //    _fileLogger = fileLogger;
        //}

        public RedboxManager(IConfiguration config, ILogger<RedboxManager> aLogger
            , IRedboxRequestManagerProxy redboxRequestManager, ISoapRequestHelper soapRequestHelper, IAppLogger logger)
        {
            _config = config;
            //_logger = logger;
            _redboxRequestManager = redboxRequestManager;
            _soapRequestHelper = soapRequestHelper;
            _logger = logger;
        }

        //protected IHttpClientProvider GetHttpClientProvider(string soapAction)
        //{
        //    return GenericRedboxHttpClientProvider.Factory(_config["Appsettings:RedboxAuthorizationId"], _config["Appsettings:RedboxModuleId"],
        //       soapAction,
        //       _httpContextUtil.GetComputerName(), "n/a", "n/a");
        //}

        private string BuildGetCustomerInfoByAcctNumPayload(string accountNumber)
        {
            var reqTranId = Util.TimeStampCode();

            string payload = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:soap=""http://soap.request.manager.redbox.stanbic.com/"">
                               <soapenv:Header/>
                               <soapenv:Body>
                                  <soap:request>
                                     <reqTranId>{reqTranId}</reqTranId>
                                     <channel>INTERNET_BANKING</channel>
                                     <type>CIF_ENQUIRY</type>
                                     <submissionTime>{DateTime.Now.ToString("o")}</submissionTime>
                                     <body><![CDATA[<otherRequestDetails>
                                     <cifId></cifId>
                                     <cifType></cifType>
                                    <accountNumber>{accountNumber}</accountNumber>
                                    <moduleTranReferenceId>{reqTranId}</moduleTranReferenceId>
                                  </otherRequestDetails>]]></body>
                                  </soap:request>
                               </soapenv:Body>
                            </soapenv:Envelope>";
            return payload;
        }

        public async Task<ValidateCustomerResponseDTO> GetCustomerInfoByAccountNumberAsync(string accountNumber)
        {
            try
            {
                var requestPayload = BuildGetCustomerInfoByAcctNumPayload(accountNumber);
                var response = await _redboxRequestManager.Post<string>(requestPayload);

                if (response.ResponseCode == "00" || response.ResponseCode == "000" || response.ResponseCode == "202")
                {
                    return BuildAccountEnquiryInfoFromResponse(response.Detail);
                }
                //return null;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occured while fetching customer profile", ex: ex);
                throw;
            }

            return default;
        }

        private ValidateCustomerResponseDTO BuildAccountEnquiryInfoFromResponse(string detail)
        {
            return new ValidateCustomerResponseDTO
            {
                FirstName = Util.GetFirstTagValue(detail, "FirstName", ignoreCase: false),
                Lastname = Util.GetFirstTagValue(detail, "LastName", ignoreCase: false),
                PhoneNumber1 = Util.GetTagValue(detail, "PhoneNumber1"),
                PhoneNumber2 = Util.GetTagValue(detail, "PhoneNumber2"),
                CustomerId = Util.GetTagValue(detail, "CustomerId"),
                AccountScheme = Util.GetTagValue(detail, "AccountSchemeDescription"),
                AccountSchemeCode = Util.GetTagValue(detail, "AccountSchemeCode")
            };
        }

        public async Task<(bool status, string otpReference)> SendOtpAsync(string accountNumber)
        {
            //Guard.IsNull(accountNumber, "accountNumber cannot be null");

            try
            {
                string payloadBody2 = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:soap=""http://soap.request.manager.redbox.stanbic.com/"">
                             <soapenv:Header/>
                                <soapenv:Body>
                                    <soap:request>
                                        <channel>MOBILE_APP</channel>
                                        <type>OTP_REQUEST</type>
                                        <customerId>{accountNumber}</customerId>
                                        <customerIdType>ACCOUNT_NUMBER</customerIdType>
                                        <body/>
                                    </soap:request>
                                </soapenv:Body>
                            </soapenv:Envelope>";

                string payloadBody1 = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:soap=""http://soap.request.manager.redbox.stanbic.com/"">
                                <soapenv:Header/>
                                <soapenv:Body>
                                <soap:request>
                                <channel>ATM</channel>
                                <type>OTP_REQUEST</type>
                                <customerId>{accountNumber}</customerId>
                                <customerIdType>ACCOUNT_NUMBER</customerIdType>
                                <body><![CDATA[<initiateOTPRequest>
                                <OTP_Type>BOTH</OTP_Type>
                                <reasonDescription>creating both PIN</reasonDescription>
                                </initiateOTPRequest>]]></body>
                                </soap:request>
                                </soapenv:Body>
                                </soapenv:Envelope>";

                string payloadBody = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:soap=""http://soap.request.manager.redbox.stanbic.com/"">
                    <soapenv:Header/>
                    <soapenv:Body>
                        <soap:request>
                            <channel>ATM</channel>
                            <type>OTP_REQUEST</type>
                            <customerId>{accountNumber}</customerId >
                            <customerIdType>ACCOUNT_NUMBER</customerIdType>
                            <body>
                                <![CDATA[<initiateOTPRequest>
                                    <OTP_Type>CUSTOM</OTP_Type>
                                    <reasonCode>99</reasonCode>
                                    <reasonDescription>Transfer</reasonDescription>
                                </initiateOTPRequest>]]>
                            </body>
                        </soap:request>
                    </soapenv:Body>
                    </soapenv:Envelope>";

                var response = await _redboxRequestManager.Post<string>(payloadBody2);
                if (response.ResponseCode == "00" || response.ResponseCode == "000")
                {
                    var reference = Util.GetTagValue(response.Detail, "reference");
                    return (true, reference);
                }
                return (false, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.Error($"ReqMngr Initiate Otp request failed with error -> {ex.Message}", ex: ex);
                throw;
            }
        }

        private string BuildReqManagerOtpVerificationRequestPayload(string accountNumber, string otp, string sourceReference)
        {
            var payload = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:soap=""http://soap.request.manager.redbox.stanbic.com/"">
                             <soapenv:Header/>
                                <soapenv:Body>
                                    <soap:request>
                                        <channel>MOBILE_APP</channel>
                                        <type>OTP_VALIDATION</type>
                                        <customerId>{accountNumber}</customerId>
                                        <customerIdType>ACCOUNT_NUMBER</customerIdType>
                                        <body>{otp}B{sourceReference}</body>
                                    </soap:request>
                                </soapenv:Body>
                            </soapenv:Envelope>";
            return payload;
        }

        public async Task<bool> ValidateOtpAsync(ValidateOTP payload)
        {
            try
            {
                var requestPayload = BuildReqManagerOtpVerificationRequestPayload(payload.AccountNumber, payload.Otp, payload.OtpRefence);

                var response = await _redboxRequestManager.Post<string>(requestPayload);

                if (response.ResponseCode == "00" || response.ResponseCode == "000")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error($"ReqMngr Initiate Otp request failed with error -> {ex.Message}", ex: ex);
                throw;
            }
        }

        public async Task<List<LoanAccountResponseDTO>> FetchLoanAccounts(string cifId)
        {
            try
            {
                var payload = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:soap=""http://soap.request.manager.redbox.stanbic.com/"">
                             <soapenv:Header/>
                                <soapenv:Body>
                                    <soap:request>
                                        <channel>MOBILE_APP</channel>
                                        <type>FETCH_ACCOUNTS</type>
                                        <customerId>{cifId}</customerId>
                                        <customerIdType>CIF_ID</customerIdType>
                                        <submissionTime>07-FEB-17 05.31.17</submissionTime>
                                        <reqTranId>218805913</reqTranId>
                                        <body><![CDATA[<otherRequestDetails><passId>{cifId}</passId><passIdType>CIF_ID</passIdType ><passCodeType>01</passCodeType><passCode>{cifId}</passCode><moduleTranReferenceId>599324090</moduleTranReferenceId><cifType></cifType></otherRequestDetails>]]></body>
                                    </soap:request>
                                </soapenv:Body>
                            </soapenv:Envelope>";

                var response = await _redboxRequestManager.Post<string>(payload);

                var hg = response.Detail;

                if (response.ResponseCode == "00" || response.ResponseCode == "000")
                {
                    //return true;
                    var accountsString = Util.GetTagValue(response.Detail, "accounts");

                    accountsString = $@"<accounts>{accountsString}</accounts>";
                    List<LoanAccountResponseDTO> accounts;

                    var loanAccountList = DeserializeXML<RedboxLoanAccountList>(accountsString);
                    accounts = loanAccountList?.LoanAccts?.Select(b => new LoanAccountResponseDTO { AccountName = b.accountName, AccountNumber = b.accountNumber, AccountCategory = b.accountCategory })?.ToList();

                    return accounts.Any() ? accounts : default;
                }
                _logger.Error("Failure Response: " + response.ResponseDescription, "Fetch Loan Account");
            }
            catch (Exception ex)
            {
                _logger.Error("Exception: " + ex.Message, "fetch Loan Accounts");
            }

            return default;
        }

        public async Task<LoanRepaymentDetailDTO> LoanAccountEnquiry(string accountNumber)
        {
            try
            {
                var payload = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:soap=""http://soap.request.manager.redbox.stanbic.com/"">
                             <soapenv:Header/>
                                <soapenv:Body>
                                    <soap:request>
                                        <reqTranId>{GeneralHelpers.GenerateRequestTranID()}</reqTranId>
                                        <channel>BPM</channel>
                                        <type>LOAN_ACCOUNT_ENQUIRY</type>
                                        <body><![CDATA[<otherRequestDetails>
                                        <accountNumber>{accountNumber}</accountNumber>
                                        <moduleTranReferenceId >{GeneralHelpers.GenerateRequestTranID()}</moduleTranReferenceId>
                                        </otherRequestDetails>]]></body>
                                        <submissionTime>2018-03-06T17:12:39.669+01:00</submissionTime>
                                    </soap:request>
                                </soapenv:Body>
                            </soapenv:Envelope>";

                var response = await _redboxRequestManager.Post<string>(payload);

                if (response.ResponseCode == "00" || response.ResponseCode == "000" || response.ResponseCode == "202")
                {
                    return BuildLoanResponseDetail(response.Detail);
                }

                _logger.Error("Failure Response: " + response, "GetCustomerInfoByAccountNumberAsync");
            }
            catch (Exception ex)
            {
                _logger.Error("Exception: " + ex.Message, "fetch Loan enquiry");
            }

            return default;
        }

        private LoanRepaymentDetailDTO BuildLoanResponseDetail(string detail)
        {
            return new LoanRepaymentDetailDTO
            {
                AccountName = Util.GetTagValue(detail, "AccountName"),
                AccountNumber = Util.GetTagValue(detail, "AccountNumber"),
                AvailableBalance = Util.GetTagValue(detail, "AvailableBalance"),
                LoanAmountValue = Util.GetTagValue(detail, "LoanAmountValue"),
                OutstandingBalance = Util.GetTagValue(detail, "OutstandingBalance"),
                AccountSchemeType = Util.GetTagValue(detail, "AccountSchemeType"),
                AccountSchemeCode = Util.GetTagValue(detail, "AccountSchemeCode"),
                AccountType = Util.GetTagValue(detail, "AccountType")
            };
        }

        private T DeserializeXML<T>(string objectData)
        {
            objectData = objectData.Replace("\n", "");
            var serializer = new XmlSerializer(typeof(T));
            object result;
            using (TextReader reader = new StringReader(objectData))
            {
                result = serializer.Deserialize(reader);
            }
            return (T)result;
        }

        public async Task SendEmailAsync(string fromEmail, string toEmail, string subject, string emailBody, string ccEmail = default)
        {
            try
            {
                var payload2 = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:soap=""http://soap.messaging.outbound.redbox.stanbic.com/"">
                           <soapenv:Header />
                               <soapenv:Body>
                               <soap:EMail>
                               <From>{fromEmail}</From>
                               <To>{toEmail}</To>
                                  <Cc/>
                                     <BCc/>
                                     <Attachments> </Attachments>
                                     <Subject>{subject}</Subject>
                                     <ContentType>text/html</ContentType>
                                     <Body>
                                        <![CDATA[{emailBody}]]></Body>
                                  </soap:EMail>
                                </soapenv:Body></soapenv:Envelope>";

                var payload = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" " +
                    " xmlns:soap=\"http://soap.messaging.outbound.redbox.stanbic.com/\">" +
                    "<soapenv:Header/><soapenv:Body><soap:EMail>" +
                    $"<From>{fromEmail}</From>" +
                    $"<To>{toEmail}</To>" +
                    $"<Cc>{ccEmail}</Cc>" +
                    "<BCc/>" +
                    "<ListSeparator>?</ListSeparator>" +
                    "<Attachments>" +
                    "<Attachment><AttachmentId>?</AttachmentId><AttachmentName>?</AttachmentName>" +
                    "<AttachmentContentType>?</AttachmentContentType><AttachmentData" +
                    ">?</AttachmentData>" +
                    "</Attachment></Attachments>" +
                    $"<Subject>{subject}</Subject>" +
                    "<ContentType>text/html</ContentType>" +
                    $"<Body><![CDATA[{emailBody}]]></Body>" +
                    "<IsSecuredFlag>0</IsSecuredFlag>" +
                    "</soap:EMail></soapenv:Body></soapenv:Envelope>";

                _logger.Info("Payload: " + payload2, "RedboxManager_SendEmailAsync");

                var res = await _soapRequestHelper.SoapCall(payload2, "sendEmailMessage", _config["AppSettings:RedboxMessagingOutboundURL"]);

                //res.ResponseCode

                //var response = await _redboxRequestManager.Post<string>(payload2);

                if (res.ResponseCode == "00" || res.ResponseCode == "000" || res.ResponseCode == "202")
                {
                    _logger.Info("Success Response: " + res.ResponseDescription, "RedboxManager_SendEmailAsync");
                }
                else
                {
                    _logger.Info("Failure Response: " + res.ResponseDescription, "RedboxManager_SendEmailAsync");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception: " + ex.Message, "RedboxManager_SendEmailAsync");
            }
        }

        public async Task SendSMSAsync(string accountNum, string message, string recipientNum)
        {
            try
            {
                var payload = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" " +
                    " xmlns:soap=\"http://soap.messaging.outbound.redbox.stanbic.com/\">" +
                    "<soapenv:Header/><soapenv:Body><soap:SMSRequest>" +
                    "<SMSList>" +
                    "<SMS>" +
                    $"<AccountNumber>{accountNum}</AccountNumber>" +
                    "<ChargeCustomer>0</ChargeCustomer>" +
                    "<CostCentre>99989</CostCentre>" +
                    "<EntityCode>11289</EntityCode>" +
                    $"<Message>{message}</Message>" +
                    "<RecipientList>" +
                    $"<RecipientMobileNumber>{recipientNum}</RecipientMobileNumber>" +
                    "</RecipientList>" +
                    "<SenderId>SIBTC</SenderId>" +
                    "<UseSenderId>1</UseSenderId>" +
                    "</SMS></SMSList>" +
                    "</soap:SMSRequest></soapenv:Body></soapenv:Envelope>";

                _logger.Info("Payload: " + payload, "RedboxManager_SendSMSAsync");

                var response = await _redboxRequestManager.Post<string>(payload);

                if (response.ResponseCode == "00" || response.ResponseCode == "000" || response.ResponseCode == "202")
                {
                    _logger.Info("Success Response: " + response.ResponseDescription, "RedboxManager_SendSMSAsync");
                }
                else
                {
                    _logger.Info("Failure Response: " + response.ResponseDescription, "RedboxManager_SendSMSAsync");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception: " + ex.Message, "RedboxManager_SendSMSAsync");
            }
        }

        public async Task<CustomerInfoDTO> GetCustomerContactInfoByAccountNumberAsync(string accountNumber)
        {
            try
            {
                string payload1 = BuildGetCustomerInfoByAcctNumPayload(accountNumber);
                string payload = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:soap=""http://soap.request.manager.redbox.stanbic.com/"">
                               <soapenv:Header/>
                               <soapenv:Body>
                                  <soap:request>
                                     <reqTranId>{GeneralHelpers.GenerateRequestTranID()}</reqTranId>
                                     <channel>INTERNET_BANKING</channel>
                                     <type>CIF_ENQUIRY</type>
                                     <submissionTime>{DateTime.Now.ToString("o")}</submissionTime>
                                     <body><![CDATA[<otherRequestDetails>
                                     <cifId></cifId>
                                     <cifType></cifType>
                                    <accountNumber>{accountNumber}</accountNumber>
                                    <moduleTranReferenceId>{GeneralHelpers.GenerateRequestTranID()}</moduleTranReferenceId>
                                  </otherRequestDetails>]]></body>
                                  </soap:request>
                               </soapenv:Body>
                            </soapenv:Envelope>";

                string payload2 = "<soap:request><channel>AGENT_NAME</channel><type>CIF_ENQUIRY</type><body>" +
                    $"<![CDATA[<otherRequestDetails><cifId></cifId><accountNumber>{accountNumber}</accountNumber>" +
                    $"<moduleTranReferenceId>{GeneralHelpers.GenerateRequestTranID()}</moduleTranReferenceId><cifType></cifType>" +
                    "</otherRequestDetails>]]>" +
                    $"</body><submissionTime>07-FEB-17 05.31.17</submissionTime><reqTranId>{GeneralHelpers.GenerateRequestTranID()}</reqTranId></soap:request>";

                var response = await _redboxRequestManager.Post<string>(payload1);

                if (response.ResponseCode == "00" || response.ResponseCode == "000" || response.ResponseCode == "202")
                {
                    var accountSegment = Util.GetFirstTagValue(response.Detail, "segmentName");
                    return new CustomerInfoDTO
                    {
                        Firstname = Util.GetFirstTagValue(response.Detail, "FirstName"),
                        Email = Util.GetFirstTagValue(response.Detail, "Email"),
                        PhoneNumber = Util.GetTagValue(response.Detail, "PhoneNumber1")
                    };
                }
                else
                {
                    _logger.Error("Failure Response: " + response.ResponseDescription, "GetCustomerInfoByAccountNumberAsync");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception: " + ex.Message, "GetCustomerInfoByAccountNumberAsync");
            }

            return default;
        }

        public async Task<(string responseCode, string responseDescription, string segment)> GetAccountSegment(string accountNumber)
        {
            try
            {
                string payload =
                    $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:soap=""http://soap.request.manager.redbox.stanbic.com/"">
                               <soapenv:Header/>
                                <soapenv:Body>
                                    <soap:request>
                                        <reqTranId>{Util.TimeStampCode()}</reqTranId>
                                        <channel>INTERNET_BANKING</channel>
                                        <type>GET_ACCOUNT_SEGMENT</type>
                                        <customerId>{accountNumber}</customerId>
                                        <customerIdType>ACCOUNT_NUMBER</customerIdType>
                                        <submissionTime>{DateTime.Now:f}</submissionTime>
                                        <body></body>
                                     </soap:request>
                               </soapenv:Body>
                              </soapenv:Envelope>";

                var response = await _redboxRequestManager.Post<string>(payload);

                if (response.ResponseCode == "00" || response.ResponseCode == "000" || response.ResponseCode == "202")
                {
                    var accountSegment = Util.GetXmlTagValue(response.Detail, "segmentName");
                    return ("00", response.ResponseDescription, accountSegment);
                }
                return (response.ResponseCode, response.ResponseDescription, null);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occurred while fetching customer profile", ex: ex);
                throw;
            }
        }

        public async Task<(bool, string)> DoStraightThroughSave(LoanRepaymentRequest payload)
        {
            try
            {
                var xmlpayload = GetLoanrepaymentPayload(payload);
                _logger.Info("Payload: " + xmlpayload, "RedboxManager_LoanRepayment");

                var response = await _redboxRequestManager.Post<string>(xmlpayload);

                if (response.ResponseCode == "00" || response.ResponseCode == "000" || response.ResponseCode == "202")
                {
                    return (true, response.ResponseDescription);
                }

                return (false, response.ResponseDescription);
            }
            catch (Exception e)
            {
                _logger.Error("Exception occurred while fetching customer profile", ex: e);
                return (false, "An error Occurred");
            }
        }

        public string GetLoanrepaymentPayload(LoanRepaymentRequest payload)
        {
            if (payload.RepaymentPlan == "Partial Repayment")
            {
                return $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:soap=""http://soap.request.manager.redbox.stanbic.com/"">
                    <soapenv:Header/>
                        <soapenv:Body>
                            <soap:request>
                                <reqTranId>{GeneralHelpers.GenerateRequestTranID()}</reqTranId>
                                <channel>BPM</channel>
                                <type>LOAN_UNSCHEDULED_REPAYMENT</type>
                                <customerId>{payload.RepaymentAcctNo}</customerId>
                                <customerIdType>OPERATING_ACCOUNT</customerIdType>
                                <submissionTime>12-May-20 16.59:15</submissionTime>
                                <body><![CDATA[
                                    <otherRequestDetails>
                                    <LoanAccount>{payload.LoanAccountNo}</LoanAccount>
                                    <OperatingAccount>{payload.RepaymentAcctNo}</OperatingAccount>
                                    <ValueDate>{DateTime.Now.ToString("dd-MM-yyyy 00:00:00", CultureInfo.InvariantCulture)}</ValueDate>
                                    <CurrencyCode>NGN</CurrencyCode>
                                    <TransactionAmount>{payload.RepaymentAmount.Replace(",", "")}</TransactionAmount>
                                    </otherRequestDetails>]]>
                                </body>
                            </soap:request>
                        </soapenv:Body>
                    </soapenv:Envelope>";

                //return $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:soap=""http://soap.request.manager.redbox.stanbic.com/"">
                //    <soapenv:Header/>
                //        <soapenv:Body>
                //            <soap:request>
                //                <reqTranId>{GeneralHelpers.GenerateRequestTranID()}</reqTranId>
                //                <channel>BPM</channel>
                //                <type>LOAN_UNSCHEDULED_REPAYMENT</type>
                //                <customerId>{payload.AccountNumber}</customerId>
                //                <customerIdType>OPERATING_ACCOUNT</customerIdType>
                //                <submissionTime>12-May-20 16.59:15</submissionTime>
                //                <body><![CDATA[
                //                    <otherRequestDetails>
                //                    <LoanAccount>{payload.LoanAccountNo}</LoanAccount>
                //                    <OperatingAccount>{payload.RepaymentAcctNo}</OperatingAccount>
                //                    <ValueDate>12-05-2020 16:59:15</ValueDate>
                //                    <CurrencyCode>NGN</CurrencyCode>
                //                    <TransactionAmount>{payload.RepaymentAmount.Replace(",", "")}</TransactionAmount>
                //                    </otherRequestDetails>]]>
                //                </body>
                //            </soap:request>
                //        </soapenv:Body>
                //    </soapenv:Envelope>";
            }

            return $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:soap=""http://soap.request.manager.redbox.stanbic.com/"">
                    <soapenv:Header/>
                        <soapenv:Body>
                            <soap:request>
                                <reqTranId>{GeneralHelpers.GenerateRequestTranID()}</reqTranId>
                                <channel>BPM</channel>
                                <type>LOAN_FULL_LIQUIDATION</type>
                                <customerId>{payload.RepaymentAcctNo}</customerId>
                                <customerIdType>OPERATING_ACCOUNT</customerIdType>
                                <submissionTime>05-May-20 16.59:15</submissionTime>
                                <body><![CDATA[
                                    <otherRequestDetails>
                                    <LoanAccount>{payload.LoanAccountNo}</LoanAccount>
                                    <OperatingAccount>{payload.RepaymentAcctNo}</OperatingAccount>
                                    <ValueDate>{DateTime.Now.ToString("dd-MM-yyyy 00:00:00", CultureInfo.InvariantCulture)}</ValueDate>
                                    <CurrencyCode>NGN</CurrencyCode>
                                    <TransactionAmount>{payload.RepaymentAmount.Replace(",", "")}</TransactionAmount>
                                    </otherRequestDetails>]]>
                                </body>
                            </soap:request>
                        </soapenv:Body>
                    </soapenv:Envelope>";
        }

        public async Task<(bool status, string responseDescription)> ValidateBvnDob(string accountNumber, string bvn, DateTime dob)
        {
            try
            {
                var requestPayload = BuildGetCustomerInfoByAcctNumPayload(accountNumber);
                var response = await _redboxRequestManager.Post<string>(requestPayload);

                bool status = true;
                var description = "";

                if (response.ResponseCode == "00" || response.ResponseCode == "000" || response.ResponseCode == "202")
                {
                    var data = BuildBvnDobValidationInfoFromResponse(response.Detail);

                    if (!ValidateBvn(bvn, data.Bvn))
                    {
                        status = false;
                        description = RESPONSE_DESCRIPTION.INVALID_BVN;
                    }

                    if (!ValidateDob(dob, data.Dob))
                    {
                        status = false;
                        description = RESPONSE_DESCRIPTION.INVALID_DOB;
                    }

                    if (!ValidateBvn(bvn, data.Bvn) && !ValidateDob(dob, data.Dob))
                    {
                        status = false;
                        description = RESPONSE_DESCRIPTION.INVALID_BVN_AND_DOB;
                    }
                }
                return (status, description);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occured while fetching customer profile", ex: ex);
                throw;
            }
        }

        public bool ValidateBvn(string first, string second)
        {
            return first == second;
        }

        public bool ValidateDob(DateTime first, DateTime second)
        {
            return (first.Year == second.Year) &&
                (first.Month == second.Month) &&
                (first.Day == second.Day);
        }

        public ValidateBvnDobResponseDto BuildBvnDobValidationInfoFromResponse(string detail)
        {
            return new ValidateBvnDobResponseDto
            {
                Bvn = Util.GetTagValue(detail, "Bvn").Length <= 11 ? Util.GetTagValue(detail, "Bvn") : Util.GetTagValue(detail, "Bvn").Substring(0, 11),
                Dob = Convert.ToDateTime(Util.GetFirstTagValue(detail, "DateOfBirth", ignoreCase: false)),
            };
        }
    }
}