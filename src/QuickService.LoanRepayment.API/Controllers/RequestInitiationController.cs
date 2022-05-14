using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuickService.LoanRepayment.API.Filters;
using QuickService.LoanRepayment.Core.Constants;
using QuickService.LoanRepayment.Core.DTO.APIRequest;
using QuickService.LoanRepayment.Core.DTO.APIResponse;
using QuickService.LoanRepayment.Core.Interfaces;
using QuickService.LoanRepayment.Infrastructure.Helpers.Interfaces;

//using SharedKernel.Interfaces;

namespace QuickService.LoanRepayment.API.Controllers
{
    //[ApiController]
    [Route("api/request-manager")]
    public class RequestInitiationController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly IRedboxManager _redboxManager;
        private readonly IRequestService _requestService;
        private readonly ILogger<RequestInitiationController> _logger;
        private readonly IAuditLogService _auditLogService;

        public RequestInitiationController(IConfiguration config, IRedboxManager redboxManager,
            IRequestService requestService,
            ILogger<RequestInitiationController> logger, IAuditLogService auditLogService)

        {
            _config = config;
            _redboxManager = redboxManager;
            _requestService = requestService;
            _logger = logger;
            _auditLogService = auditLogService;
        }

        [Route("ValidateAccountNoAndPhone")]
        //[ProducesResponseType(200, Type = typeof(GenericAPIResponse<string>))]
        //[ProducesResponseType(400)]
        [HttpPost]
        public async Task<IActionResult> ValidateAccountNoAndPhone([FromBody] ValidateCustomerRequest payload)
        {
            ValidateCustomerResponseDTO data = new ValidateCustomerResponseDTO();

            try
            {
                await _auditLogService.AuditLog(payload.AccountNo, "Loan Repayment", "ValidateAccountNoAndPhone", "Commencing the validation of Customer account details: ", HttpContext.Connection.RemoteIpAddress.ToString(), Dns.GetHostName().ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured on api/request-manager/ValidateAccountNoAndPhone -> {ex.Message}", ex);
                return Ok(new GenericAPIResponse<object>(RESPONSE_CODE.FAILURE, ex.Message, null));
            }

            try
            {
                var response = await _requestService.ValidateCustomerAndReturnCustomerDetailsAsync(payload);

                if (response.ResponseCode == RESPONSE_CODE.FAILURE)
                {
                    return Ok(new GenericAPIResponse<ValidateCustomerResponseDTO>()
                    {
                        ResponseCode = response.ResponseCode,
                        ResponseDescription = response.ResponseDescription,
                        Data = response.Data
                    });
                }
                else
                {
                    var acctSegmentResponse = await _redboxManager.GetAccountSegment(payload.AccountNo);
                    if (acctSegmentResponse.responseCode == RESPONSE_CODE.SUCCESS)
                    {
                        data = new ValidateCustomerResponseDTO()
                        {
                            FirstName = response.Data.FirstName,
                            Lastname = response.Data.Lastname,
                            AccountScheme = response.Data.AccountScheme,
                            AccountSchemeCode = response.Data.AccountSchemeCode,
                            CustomerId = response.Data.CustomerId,
                            PhoneNumber1 = response.Data.PhoneNumber1,
                            PhoneNumber2 = response.Data.PhoneNumber2,
                            AccountSegment = acctSegmentResponse.segment
                        };
                        return Ok(new GenericAPIResponse<ValidateCustomerResponseDTO>()
                        {
                            ResponseCode = RESPONSE_CODE.SUCCESS,
                            ResponseDescription = RESPONSE_DESCRIPTION.REQUEST_PROCESSED_SUCCESSFULLY,
                            Data = data
                        });
                    }
                    else
                    {
                        data = null;
                        return Ok(new GenericAPIResponse<ValidateCustomerResponseDTO>()
                        {
                            ResponseCode = RESPONSE_CODE.FAILURE,
                            ResponseDescription = RESPONSE_DESCRIPTION.GENERAL_FAILURE,
                            Data = data
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), ex.Message);
                return Ok(new GenericAPIResponse()
                {
                    ResponseCode = RESPONSE_CODE.FAILURE,
                    ResponseDescription = RESPONSE_DESCRIPTION.GENERAL_FAILURE
                });
            }
        }

        [HttpPost]
        [Route("SendOTP")]
        [ServiceFilter(typeof(ModelStateValidationFilter))]
        //[ProducesResponseType(200, Type = typeof(GenericAPIResponse<string>))]
        //[ProducesResponseType(400)]
        public async Task<IActionResult> SendOTP([FromBody] SendOTPRequest request)
        {
            try
            {
                await _auditLogService.AuditLog(request.AccountNumber, "Loan Repayment", "SendOTP", "Sending OTP: ", HttpContext.Connection.RemoteIpAddress.ToString(), Dns.GetHostName().ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured on api/request-manager/SendOTP -> {ex.Message}", ex);
                return Ok(new GenericAPIResponse<object>(RESPONSE_CODE.FAILURE, ex.Message, null));
            }
            (bool status, string otpReference) = await _redboxManager.SendOtpAsync(request.AccountNumber);

            return Ok(new GenericAPIResponse<string>()
            {
                ResponseCode = status ? RESPONSE_CODE.SUCCESS : RESPONSE_CODE.FAILURE,
                ResponseDescription = status ? RESPONSE_DESCRIPTION.REQUEST_PROCESSED_SUCCESSFULLY : RESPONSE_DESCRIPTION.GENERAL_FAILURE,
                Data = otpReference
            });
        }

        [HttpPost]
        [Route("ValidateOTP")]
        [ServiceFilter(typeof(ModelStateValidationFilter))]
        //[ProducesResponseType(200, Type = typeof(GenericAPIResponse<string>))]
        //[ProducesResponseType(400)]
        public async Task<IActionResult> ValidateOTP([FromBody] ValidateOTP payload)
        {
            try
            {
                await _auditLogService.AuditLog(payload.AccountNumber, "Loan Repayment", "SendOTP", "Sending OTP: ", HttpContext.Connection.RemoteIpAddress.ToString(), Dns.GetHostName().ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured on api/request-manager/SendOTP -> {ex.Message}", ex);
                return Ok(new GenericAPIResponse<object>(RESPONSE_CODE.FAILURE, ex.Message, null));
            }
            try
            {
                var Validated = await _redboxManager.ValidateOtpAsync(payload);
                if (Validated)
                {
                    return Ok(new GenericAPIResponse()
                    {
                        ResponseCode = RESPONSE_CODE.SUCCESS,
                        ResponseDescription = RESPONSE_DESCRIPTION.OTP_VALIDATION_SUCCESSFUL
                    });
                }
                else
                {
                    return Ok(new GenericAPIResponse()
                    {
                        ResponseCode = RESPONSE_CODE.FAILURE,
                        ResponseDescription = RESPONSE_DESCRIPTION.OTP_VALIDATION_FAILED
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), ex.Message);
                return Ok(new GenericAPIResponse()
                {
                    ResponseCode = RESPONSE_CODE.FAILURE,
                    ResponseDescription = RESPONSE_DESCRIPTION.GENERAL_FAILURE
                });
            }
        }

        [HttpPost]
        [Route("LoanRepaymentDetails")]
        [ServiceFilter(typeof(ModelStateValidationFilter))]
        //[ProducesResponseType(200, Type = typeof(GenericAPIResponse<LoanRepaymentDetailDTO>))]
        //[ProducesResponseType(400)]
        public async Task<IActionResult> LoanRepaymentDetails([FromBody] LoanAccountRequest payload)
        {
            try
            {
                await _auditLogService.AuditLog(payload.CifId, "Loan Repayment", "LoanRepaymentDetails", "Fetching Loan Repayment Details: ", HttpContext.Connection.RemoteIpAddress.ToString(), Dns.GetHostName().ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured on api/request-manager/LoanRepaymentDetails -> {ex.Message}", ex);
                return Ok(new GenericAPIResponse<object>(RESPONSE_CODE.FAILURE, ex.Message, null));
            }
            try
            {
                var loanAcctResponse = new List<LoanAccountResponseDTO>();
                var loanAcctDetails = new LoanRepaymentDetailDTO();
                var loanAcctRepaymentDetails = new List<LoanRepaymentDetailDTO>();

                loanAcctResponse = await _redboxManager.FetchLoanAccounts(payload.CifId);

                if (loanAcctResponse.Count > 0)
                {
                    foreach (var acct in loanAcctResponse)
                    {
                        loanAcctDetails = await _redboxManager.LoanAccountEnquiry(acct.AccountNumber);
                        loanAcctRepaymentDetails.Add(loanAcctDetails);
                    }

                    return Ok(new GenericAPIResponse<List<LoanRepaymentDetailDTO>>()
                    {
                        ResponseCode = RESPONSE_CODE.SUCCESS,
                        ResponseDescription = RESPONSE_DESCRIPTION.REQUEST_PROCESSED_SUCCESSFULLY,
                        Data = loanAcctRepaymentDetails
                    });
                }
                else if (loanAcctResponse.Count == 0)
                {
                    return Ok(new GenericAPIResponse<List<LoanRepaymentDetailDTO>>()
                    {
                        ResponseCode = RESPONSE_CODE.SUCCESS,
                        ResponseDescription = RESPONSE_DESCRIPTION.REQUEST_PROCESSED_SUCCESSFULLY,
                        Data = null
                    });
                }
                else
                {
                    return Ok(new GenericAPIResponse<LoanRepaymentDetailDTO>()
                    {
                        ResponseCode = RESPONSE_CODE.FAILURE,
                        ResponseDescription = RESPONSE_DESCRIPTION.GENERAL_FAILURE,
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), ex.Message);
                return Ok(new GenericAPIResponse()
                {
                    ResponseCode = RESPONSE_CODE.FAILURE,
                    ResponseDescription = RESPONSE_DESCRIPTION.GENERAL_FAILURE,
                });
            }
        }

        [Route("InitiateRequest")]
        [HttpPost]
        public async Task<IActionResult> InitiateRequest([FromBody] LoanRepaymentRequest payload)
        {
            try
            {
                await _auditLogService.AuditLog(payload.LoanAccountNo, "Loan Repayment", "InitiateRequest", "Initiating Request: ", HttpContext.Connection.RemoteIpAddress.ToString(), Dns.GetHostName().ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured on api/request-manager/InitiateRequest -> {ex.Message}", ex);
                return Ok(new GenericAPIResponse<object>(RESPONSE_CODE.FAILURE, ex.Message, null));
            }
            try
            {
                if (ModelState.IsValid)
                {
                    var (ticketId, response) = await _requestService.SubmitRequestAsync(payload);

                    if (!string.IsNullOrEmpty(ticketId))
                    {
                        return Ok(new GenericAPIResponse<InitiatedRequestDTO>()
                        {
                            ResponseCode = RESPONSE_CODE.SUCCESS,
                            ResponseDescription = RESPONSE_DESCRIPTION.REQUEST_CREATION_SUCCESS,
                            Data = new InitiatedRequestDTO()
                            {
                                TicketID = ticketId
                            }
                        });
                    }

                    return Ok(new GenericAPIResponse<InitiatedRequestDTO>()
                    {
                        ResponseCode = RESPONSE_CODE.FAILURE,
                        ResponseDescription = string.IsNullOrEmpty(response) ? RESPONSE_DESCRIPTION.FINACLE_SUBMISSION_FAILURE : response,
                        Data = new InitiatedRequestDTO()
                        {
                            TicketID = null
                        }
                    });
                }
                else
                {
                    return Ok(new GenericAPIResponse<object>()
                    {
                        ResponseCode = RESPONSE_CODE.FAILURE,
                        ResponseDescription = "Loan Request Payload is not valid",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), ex.Message);
                return Ok(new GenericAPIResponse()
                {
                    ResponseCode = RESPONSE_CODE.FAILURE,
                    ResponseDescription = RESPONSE_DESCRIPTION.GENERAL_FAILURE
                });
            }
        }

        [HttpPost]
        [Route("ConfirmBVNDateOfBirth")]
        [ServiceFilter(typeof(ModelStateValidationFilter))]
        //[ProducesResponseType(200, Type = typeof(GenericAPIResponse<string>))]
        //[ProducesResponseType(400)]
        public async Task<IActionResult> ConfirmBVNDateOfBirth([FromBody] ValidateBvnBobOTP payload)
        {
            try
            {
                await _auditLogService.AuditLog(payload.AccountNumber, "Loan Repayment", "ConfirmBVNDateOfBirth",
                    "Confirming BVN and Date of Birth: ", HttpContext.Connection.RemoteIpAddress.ToString(),
                    Dns.GetHostName().ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured on api/request-manager/ConfirmBVNDateOfBirth -> {ex.Message}", ex);
                return Ok(new GenericAPIResponse<object>(RESPONSE_CODE.FAILURE, ex.Message, null));
            }

            try
            {
                var response = await _redboxManager.ValidateBvnDob(payload.AccountNumber, payload.Bvn, payload.Dob);
                return Ok(new GenericAPIResponse<string>()
                {
                    ResponseCode = response.status ? RESPONSE_CODE.SUCCESS : RESPONSE_CODE.FAILURE,
                    ResponseDescription = response.status
                        ? RESPONSE_DESCRIPTION.REQUEST_PROCESSED_SUCCESSFULLY
                        : RESPONSE_DESCRIPTION.GENERAL_FAILURE,
                    Data = response.responseDescription
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), ex.Message);
                return Ok(new GenericAPIResponse<string>()
                {
                    ResponseCode = RESPONSE_CODE.FAILURE,
                    ResponseDescription = RESPONSE_DESCRIPTION.GENERAL_FAILURE
                });
            }
        }
    }
}