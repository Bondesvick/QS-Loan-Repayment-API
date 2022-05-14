using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuickService.LoanRepayment.API.Filters;

//using SharedKernel.Interfaces;
using QuickService.LoanRepayment.Core.DTO.APIResponse;
using QuickService.LoanRepayment.Core.DTO.APIRequest;
using QuickService.LoanRepayment.Core.Constants;
using QuickService.LoanRepayment.Core.Interfaces;

namespace QuickService.LoanRepayment.API.Controllers
{
    [Route("api/request-manager")]
    [ApiController]
    //[ServiceFilter(typeof(AuthSecretKeyFilter))]
    public class RequestManagerController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly IRequestService _requestService;
        private readonly ILogger<RequestManagerController> _logger;

        public RequestManagerController(IConfiguration config, IRequestService requestService, ILogger<RequestManagerController> logger)

        {
            _config = config;
            _requestService = requestService;
            _logger = logger;
        }

        [HttpPost]
        [Route("dated-requests")]
        [ServiceFilter(typeof(ModelStateValidationFilter))]
        [ProducesResponseType(200, Type = typeof(GenericAPIResponse<List<CustomerRequestDTO>>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetDatedRequest(GetDatedCustomerRequest request)
        {
            if (!(request.Status.Equals(REQUEST_STATUS.ALL, StringComparison.OrdinalIgnoreCase) ||
                request.Status.Equals(REQUEST_STATUS.ASSIGNED, StringComparison.OrdinalIgnoreCase) ||
                request.Status.Equals(REQUEST_STATUS.DECLINED, StringComparison.OrdinalIgnoreCase) ||
                request.Status.Equals(REQUEST_STATUS.PENDING, StringComparison.OrdinalIgnoreCase)
                || request.Status.Equals(REQUEST_STATUS.RESOLVED, StringComparison.OrdinalIgnoreCase)))
                return BadRequest(RESPONSE_DESCRIPTION.INVALID_REQUEST_STATUS);

            try
            {
                DateTime firstAppDeploymentDate = Convert.ToDateTime(_config["AppSettings:FirstAppDeploymentDate"],
               CultureInfo.CreateSpecificCulture("en-GB"));

                if (request.FromDate.Date < firstAppDeploymentDate.Date || request.ToDate.Date < firstAppDeploymentDate.Date)
                    return BadRequest(RESPONSE_DESCRIPTION.INVALID_REQUEST_DATE);

                if (request.FromDate.Date > request.ToDate.Date)
                    return BadRequest(RESPONSE_DESCRIPTION.INVALID_TO_DATE_FROM_DATE);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception: " + ex.Message, "GetDatedRequest");
                return BadRequest(RESPONSE_DESCRIPTION.DATE_PARSING_FAILURE);
            }

            List<CustomerRequestDTO> requests = await _requestService.GetCustomerRequestLoggedByDateAsync(request);

            return Ok(new GenericAPIResponse<List<CustomerRequestDTO>>()
            {
                ResponseCode = requests is null ? RESPONSE_CODE.FAILURE : RESPONSE_CODE.SUCCESS,
                ResponseDescription = requests is null ? RESPONSE_DESCRIPTION.GENERAL_FAILURE : RESPONSE_DESCRIPTION.REQUEST_PROCESSED_SUCCESSFULLY,
                Data = requests
            });
        }

        [HttpPost]
        [Route("my-dated-requests")]
        [ServiceFilter(typeof(ModelStateValidationFilter))]
        [ProducesResponseType(200, Type = typeof(GenericAPIResponse<List<CustomerRequestDTO>>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetMyDatedRequest(GetDatedCustomerRequest request)
        {
            if (!(request.Status.Equals(REQUEST_STATUS.ALL, StringComparison.OrdinalIgnoreCase) ||
                request.Status.Equals(REQUEST_STATUS.ASSIGNED, StringComparison.OrdinalIgnoreCase) ||
                request.Status.Equals(REQUEST_STATUS.DECLINED, StringComparison.OrdinalIgnoreCase) ||
                request.Status.Equals(REQUEST_STATUS.PENDING, StringComparison.OrdinalIgnoreCase)
                || request.Status.Equals(REQUEST_STATUS.RESOLVED, StringComparison.OrdinalIgnoreCase)))
                return BadRequest(RESPONSE_DESCRIPTION.INVALID_REQUEST_STATUS);

            try
            {
                DateTime firstAppDeploymentDate = Convert.ToDateTime(_config["AppSettings:FirstAppDeploymentDate"],
               CultureInfo.CreateSpecificCulture("en-GB"));

                if (request.FromDate.Date < firstAppDeploymentDate.Date || request.ToDate.Date < firstAppDeploymentDate.Date)
                    return BadRequest(RESPONSE_DESCRIPTION.INVALID_REQUEST_DATE);

                if (request.FromDate.Date > request.ToDate.Date)
                    return BadRequest(RESPONSE_DESCRIPTION.INVALID_TO_DATE_FROM_DATE);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception: " + ex.Message, "GetDatedRequest");
                return BadRequest(RESPONSE_DESCRIPTION.DATE_PARSING_FAILURE);
            }

            if (string.IsNullOrEmpty(request.Treater))
                return BadRequest(RESPONSE_DESCRIPTION.EMPTY_SAPID);

            List<CustomerRequestDTO> requests = await _requestService.GetCustomerRequestLoggedByDateAndTreaterAsync(request);

            return Ok(new GenericAPIResponse<List<CustomerRequestDTO>>()
            {
                ResponseCode = requests is null ? RESPONSE_CODE.FAILURE : RESPONSE_CODE.SUCCESS,
                ResponseDescription = requests is null ? RESPONSE_DESCRIPTION.GENERAL_FAILURE :
                RESPONSE_DESCRIPTION.REQUEST_PROCESSED_SUCCESSFULLY,
                Data = requests
            });
        }

        [HttpPost]
        [Route("request-doc")]
        [ServiceFilter(typeof(ModelStateValidationFilter))]
        [ProducesResponseType(200, Type = typeof(GenericAPIResponse<DocumentDTO>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetRequestDocument(GetRequestDocumentAPIRequest request)
        {
            DocumentDTO doc = await _requestService.GetRequestDocumentAsync(request.RequestId, request.DocId);

            return Ok(new GenericAPIResponse<DocumentDTO>()
            {
                ResponseCode = doc is null ? RESPONSE_CODE.FAILURE : RESPONSE_CODE.SUCCESS,
                ResponseDescription = doc is null ? RESPONSE_DESCRIPTION.DOCUMENT_NOT_FOUND :
                RESPONSE_DESCRIPTION.REQUEST_PROCESSED_SUCCESSFULLY,
                Data = doc
            });
        }

        [HttpPost]
        [Route("request-stats")]
        [ServiceFilter(typeof(ModelStateValidationFilter))]
        [ProducesResponseType(200, Type = typeof(GenericAPIResponse<DocumentDTO>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CountRequestByTypeAndSapId(StatisticsAPIRequest request)
        {
            try
            {
                StatisticsDTO stats = await _requestService.GetRequestStatisticsAsync(request.SapId, request.RequestType);

                return Ok(new GenericAPIResponse<StatisticsDTO>()
                {
                    ResponseCode = stats != null ? RESPONSE_CODE.SUCCESS : RESPONSE_CODE.FAILURE,
                    ResponseDescription = stats != null ? RESPONSE_DESCRIPTION.REQUEST_PROCESSED_SUCCESSFULLY : RESPONSE_DESCRIPTION.GENERAL_FAILURE,
                    Data = stats
                });
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
        [Route("treat-request")]
        [ProducesResponseType(200, Type = typeof(GenericAPIResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> TreatCustomerRequest(TreatRequest request)
        {
            (bool validated, string validationMessage) = request.ValidationFunc();

            if (!validated)
                return BadRequest(validationMessage);

            (bool status, string statusMessage) = await _requestService.TreatCustomerRequestAsync(request);

            if (!status)
                return Ok(new GenericAPIResponse()
                {
                    ResponseCode = RESPONSE_CODE.FAILURE,
                    ResponseDescription = statusMessage
                });

#pragma warning disable

            Task.Factory.StartNew(async () =>
            {
                string message = _config["AppSettings:ResolveMessage"];

                if (request.Status.Equals(REQUEST_STATUS.DECLINED, StringComparison.OrdinalIgnoreCase))
                {
                    message = _config["AppSettings:DeclineMessage"];

                    await _requestService.NotifyCustomerAsync(request.CustomerAccountNumber, message,
                        _config["AppSettings:EmailSubject"], _config["AppSettings:SenderEmail"], true);
                }
                else
                    await _requestService.NotifyCustomerAsync(request.CustomerAccountNumber, message,
                        _config["AppSettings:EmailSubject"], _config["AppSettings:SenderEmail"]);
            });

#pragma warning restore

            return Ok(new GenericAPIResponse()
            {
                ResponseCode = RESPONSE_CODE.SUCCESS,
                ResponseDescription = RESPONSE_DESCRIPTION.REQUEST_PROCESSED_SUCCESSFULLY
            });
        }

        [Route("assign-request")]
        [HttpPost]
        public async Task<IActionResult> SelectRequestForProcessing(SelectRequestForProcessingRequest request)
        {
            try
            {
                (bool status, string statusMessage) = await _requestService.AssignRequestForProcessingAsync(request.RequestId, request.SapId, request.Unit);
                return Ok(new GenericAPIResponse()
                {
                    ResponseCode = status ? RESPONSE_CODE.SUCCESS : RESPONSE_CODE.FAILURE,
                    ResponseDescription = statusMessage
                });
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
    }
}