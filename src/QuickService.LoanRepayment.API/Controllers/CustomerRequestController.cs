using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QuickService.LoanRepayment.API.Filters;
using QuickService.LoanRepayment.Core.DTO;
using QuickService.LoanRepayment.Core.Interfaces;
using QuickServiceAdmin.Core.Model;

namespace QuickService.LoanRepayment.API.Controllers
{
    [Route("api/request-manager")]
    //[ApiController]
    public class CustomerRequestController : BaseController
    {
        private readonly ILogger<CustomerRequestController> _logger;
        private readonly ICustomerRequestRepository _customerRequestRepository;

        public CustomerRequestController(ILogger<CustomerRequestController> logger, ICustomerRequestRepository customerRequestRepository)
        {
            _logger = logger;
            _customerRequestRepository = customerRequestRepository;
        }

        [HttpPost]
        [Route("GetCustomerRequest")]
        [ServiceFilter(typeof(ModelStateValidationFilter))]
        public async Task<IActionResult> GetCustomerRequest([FromBody] GetCustomerRequestParams getCustomerRequestParams)
        {
            var request = await _customerRequestRepository.GetCustomerRequest(getCustomerRequestParams);
            return Ok();
        }

        [HttpPost]
        [Route("GetCustomerRequests")]
        [ServiceFilter(typeof(ModelStateValidationFilter))]
        public async Task<IActionResult> GetCustomerRequests([FromBody] CustomerRequestFilterParams customerRequestFilterParams)
        {
            var requests = _customerRequestRepository.GetAllCustomerRequests(customerRequestFilterParams);
            return Ok();
        }
    }
}