using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using QuickService.LoanRepayment.Core.Constants;
using QuickService.LoanRepayment.Core.DTO.APIRequest;
using QuickService.LoanRepayment.Core.DTO.APIResponse;
using QuickService.LoanRepayment.Core.DTO.Services;
using QuickService.LoanRepayment.Core.Entities;
using QuickService.LoanRepayment.Core.Interfaces;
using QuickService.LoanRepayment.Core.Utils;

//using Microsoft.Extensions.Configuration;

namespace QuickService.LoanRepayment.Infrastructure.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRequestQueries _queries;
        private readonly IRequestCommands _commands;
        private readonly IRedboxManager _redboxManager;
        private readonly IConfiguration _configuration;

        public RequestService(IRequestQueries queries, IRequestCommands commands, IRedboxManager redboxManager, IConfiguration configuration)
        {
            _queries = queries;
            _commands = commands;
            _redboxManager = redboxManager;
            _configuration = configuration;
        }

        public async Task<GenericAPIResponse<ValidateCustomerResponseDTO>> ValidateCustomerAndReturnCustomerDetailsAsync(ValidateCustomerRequest request)
        {
            GenericAPIResponse<ValidateCustomerResponseDTO> response = new GenericAPIResponse<ValidateCustomerResponseDTO>();

            ValidateCustomerResponseDTO validationResponse = await _redboxManager.GetCustomerInfoByAccountNumberAsync(request.AccountNo);

            if (validationResponse == null)
            {
                response.ResponseCode = RESPONSE_CODE.FAILURE;
                response.ResponseDescription = RESPONSE_DESCRIPTION.INVALID_DETAILS;
                return response;
            }

            if (!request.PhoneNo.Equals(validationResponse.PhoneNumber1.Replace("+234(0)", "0"), StringComparison.OrdinalIgnoreCase))
            {
                response.ResponseCode = RESPONSE_CODE.FAILURE;
                response.ResponseDescription = RESPONSE_DESCRIPTION.PHONE_VALIDATION_FAILED;
                //response.Data = validationResponse;
                return response;
            }
            else
            {
                response.ResponseCode = RESPONSE_CODE.SUCCESS;
                response.ResponseDescription = RESPONSE_DESCRIPTION.REQUEST_PROCESSED_SUCCESSFULLY;
                response.Data = validationResponse;
            }
            return response;
        }

        public async Task<List<CustomerRequestDTO>> GetCustomerRequestLoggedByDateAsync(GetDatedCustomerRequest request)
        {
            return await _queries.GetCustomerRequestsByDateAsync(request.PageSize, request.LastIdFetched, request.FromDate, request.ToDate,
                request.RequestType, request.Status.Equals(REQUEST_STATUS.ALL, StringComparison.OrdinalIgnoreCase) ? default : request.Status);
        }

        public async Task<List<CustomerRequestDTO>> GetCustomerRequestLoggedByDateAndTreaterAsync(GetDatedCustomerRequest request)
        {
            return await _queries.GetCustomerRequestsByDateAndTreaterAsync(request.PageSize, request.LastIdFetched, request.FromDate, request.ToDate,
                 request.RequestType, request.Treater, request.Status.Equals(REQUEST_STATUS.ALL, StringComparison.OrdinalIgnoreCase) ? default : request.Status);
        }

        public async Task<DocumentDTO> GetRequestDocumentAsync(long customerRequestId, long docId)
        {
            return await _queries.GetRequestDocumentAsync(customerRequestId, docId);
        }

        public async Task<StatisticsDTO> GetRequestStatisticsAsync(string sapId, string requestType)
        {
            return await _queries.GetRequestStatisticsAsync(sapId, requestType);
        }

        public async Task<(bool status, string statusMessage)> AssignRequestForProcessingAsync(long requestId, string sapId, string unit)
        {
            CustomerRequest customerRequest = await _queries.GetCustomerRequestByIdWithTrackingAsync(requestId);

            if (customerRequest is null)
                return (false, RESPONSE_DESCRIPTION.UNAUTHORIZED_OPERATION);

            if (!customerRequest.Status.Equals(REQUEST_STATUS.PENDING, StringComparison.OrdinalIgnoreCase))
                return (false, RESPONSE_DESCRIPTION.ASSIGN_ERROR);

            customerRequest.Status = REQUEST_STATUS.ASSIGNED;
            customerRequest.TreatedDate = DateTime.Now;
            customerRequest.TreatedBy = sapId.ToUpper();
            customerRequest.TreatedByUnit = unit.Trim();

            if (await _commands.UpdateCustomerRequestAsync(customerRequest))
                return (true, RESPONSE_DESCRIPTION.REQUEST_PROCESSED_SUCCESSFULLY);

            return (false, RESPONSE_DESCRIPTION.GENERAL_FAILURE);
        }

        public async Task<(bool status, string statusMessage)> TreatCustomerRequestAsync(TreatRequest request)
        {
            CustomerRequest customerRequest = await _queries.GetCustomerRequestByIdAndSapIdWithTrackingAsync(request.RequestId, request.SapId);

            if (customerRequest is null)
            {
                return (false, RESPONSE_DESCRIPTION.UNAUTHORIZED_OPERATION);
            }

            customerRequest.Status = request.Status.ToUpper();
            customerRequest.TreatedDate = DateTime.Now.AddHours(1); // DateTime.Now
            customerRequest.Remarks = request.Remarks;

            if (request.Status.Equals(REQUEST_STATUS.DECLINED, StringComparison.OrdinalIgnoreCase))
                customerRequest.RejectionReason = request.RejectionReason;

            if (await _commands.UpdateCustomerRequestAsync(customerRequest))
                return (true, customerRequest.TranId);

            return (false, RESPONSE_DESCRIPTION.GENERAL_FAILURE);
        }

        public async Task NotifyCustomerAsync(string accountNumber, string message, string emailSubject, string senderEmail,
            bool isRejection = false)
        {
            CustomerInfoDTO accountInfo = await _redboxManager.GetCustomerContactInfoByAccountNumberAsync(accountNumber);

            if (accountInfo is null)
                return;

            if (!string.IsNullOrEmpty(accountInfo.PhoneNumber))
                accountInfo.PhoneNumber = accountInfo.PhoneNumber.Replace("+", "").Replace("(0)", "");

            if (isRejection)
            {
                if (!string.IsNullOrEmpty(accountInfo.PhoneNumber))
                    await _redboxManager.SendSMSAsync(accountNumber, message, accountInfo.PhoneNumber);

                if (!string.IsNullOrEmpty(accountInfo.Email))
                    await _redboxManager.SendEmailAsync(senderEmail, accountInfo.Email, emailSubject, message);

                return;
            }

            message = message.Replace("#FirstName#", accountInfo.Firstname);

            if (!string.IsNullOrEmpty(accountInfo.PhoneNumber))
                await _redboxManager.SendSMSAsync(accountNumber, message, accountInfo.PhoneNumber);

            if (!string.IsNullOrEmpty(accountInfo.Email))
                await _redboxManager.SendEmailAsync(senderEmail, accountInfo.Email, emailSubject, message);
        }

        public async Task<(string, string)> SubmitRequestAsync(LoanRepaymentRequest payload)
        {
            CustomerRequest customerRequest = new CustomerRequest
            {
                TranId = "LR" + GeneralHelpers.GenerateRequestTranID(),
                AccountNumber = payload.AccountNumber,
                AccountName = payload.AccountName,
                //CustomerAuthType = payload.HaveDebitCard == "No" ? AppConstants.LOAN_REPAYMENT_AUTH_TYPE : AppConstants.LOAN_REPAYMENT_DEBIT_CARD_TYPE,
                CustomerAuthType = AppConstants.LOAN_REPAYMENT_AUTH_TYPE,
                Status = REQUEST_STATUS.RESOLVED,
                CreatedDate = DateTime.UtcNow.AddHours(1),
                TreatedDate = DateTime.UtcNow,
                RequestType = AppConstants.LOAN_REPAYMENT_REQUEST_TYPE_NAME,
                LoanRepaymentDetails = new LoanRepaymentDetails(),
            };

            {
                new LoanRepaymentDetails
                {
                    AccountName = payload.AccountName,
                    AccountNumber = payload.AccountNumber,
                    SignatureContent = payload.SignatureBase64,
                    SignatureExt = payload.SignatureExt,
                    AccountSegment = payload.AccountSegment,
                    Amount = payload.Amount,         ////////////
                    RepaymentPlan = payload.RepaymentPlan,
                    LoanAccountNo = payload.LoanAccountNo,
                    LoanCurrentBalance = (float.Parse(payload.LoanCurrentBalance) - float.Parse(payload.Amount.Replace(",", ""))).ToString(CultureInfo.InvariantCulture),
                    RepaymentAcctNo = payload.RepaymentAcctNo,
                    RepaymentAmount = payload.RepaymentAmount,    ///////////////
                    HaveDebitCard = payload.HaveDebitCard
                };
            }

            //customerRequest.LoanRepaymentDetails.InjectFrom(payload);

            customerRequest.LoanRepaymentDetails = new LoanRepaymentDetails
            {
                AccountName = payload.AccountName,
                AccountNumber = payload.AccountNumber,
                SignatureContent = payload.SignatureBase64,
                SignatureExt = payload.SignatureExt,
                AccountSegment = payload.AccountSegment,
                Amount = payload.Amount,         ////////////
                RepaymentPlan = payload.RepaymentPlan,
                LoanAccountNo = payload.LoanAccountNo,
                LoanCurrentBalance = (float.Parse(payload.LoanCurrentBalance) - float.Parse(payload.Amount.Replace(",", ""))).ToString(CultureInfo.InvariantCulture),
                RepaymentAcctNo = payload.RepaymentAcctNo,
                RepaymentAmount = payload.RepaymentAmount,    ///////////////
                HaveDebitCard = payload.HaveDebitCard
            };

            #region Init Document Entities

            if (!string.IsNullOrEmpty(payload.SignatureBase64))
                customerRequest.LoanRepaymentDocuments.Add(new LoanRepaymentDocument()
                {
                    DocName = "SIGNATURE",
                    DocContent = payload.SignatureBase64,
                    DocExtension = payload.SignatureExt
                });

            #endregion Init Document Entities

            //if (await _commands.LogLoanRepaymentRequestAsync(customerRequest))
            //{
            var (saved, response) = await _redboxManager.DoStraightThroughSave(payload);

            if (!saved) return (default, response);
            //customerRequest.Status = "RESOLVED";
            //await _commands.UpdateCustomerRequestAsync(customerRequest);

            await _commands.LogLoanRepaymentRequestAsync(customerRequest);
            var accountInfo = await _redboxManager.GetCustomerContactInfoByAccountNumberAsync(payload.AccountNumber);
            await SendSubmissionNotificationMessageAsync(accountInfo.Firstname, accountInfo.Email.ToLower(), customerRequest.TranId);

            //await Task.Factory.StartNew(async () =>
            //{
            //    var accountInfo = await _redboxManager.GetCustomerContactInfoByAccountNumberAsync(payload.AccountNumber);
            //    await SendSubmissionNotificationMessageAsync(accountInfo.Firstname, accountInfo.Email.ToLower(), customerRequest.TranId);
            //});

            return (customerRequest.TranId, response);

            //}

            //return (default, "");
        }

        private async Task SendSubmissionNotificationMessageAsync(string firstname, string email, string tranId)
        {
            var message = _configuration["AppSettings:SubmissionMessage"];
            message = message.Replace("#FirstName#", firstname).Replace("#Ticket#", tranId);

            await _redboxManager.SendEmailAsync(_configuration["AppSettings:SenderEmail"], email, _configuration["AppSettings:EmailSubject"], message);
        }
    }
}