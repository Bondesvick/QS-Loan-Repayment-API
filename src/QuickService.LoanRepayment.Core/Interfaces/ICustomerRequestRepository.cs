using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using QuickService.LoanRepayment.Core.DTO;
using QuickService.LoanRepayment.Core.Entities;
using QuickService.LoanRepayment.Core.Models;
using QuickServiceAdmin.Core.Model;

namespace QuickService.LoanRepayment.Core.Interfaces
{
    public interface ICustomerRequestRepository
    {
        Task<CustomerRequestDetails> GetCustomerRequest(GetCustomerRequestParams getCustomerRequestParams);

        Task<List<CustomerRequest>> GetAllCustomerRequests(CustomerRequestFilterParams customerRequestFilterParams);
    }
}