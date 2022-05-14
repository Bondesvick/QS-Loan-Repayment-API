using System.Collections.Generic;
using System.Threading.Tasks;
using QuickService.LoanRepayment.Core.DTO;
using QuickService.LoanRepayment.Core.Entities;
using QuickService.LoanRepayment.Core.Interfaces;
using QuickService.LoanRepayment.Core.Models;
using QuickService.LoanRepayment.Infrastructure.Data;
using QuickServiceAdmin.Core.Model;

namespace QuickService.LoanRepayment.Infrastructure.Services
{
    public class CustomerRequestRepository : ICustomerRequestRepository
    {
        private readonly AppDbContext _db;

        public CustomerRequestRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<CustomerRequestDetails> GetCustomerRequest(GetCustomerRequestParams getCustomerRequestParams)
        {
            var query = _db.CustomerRequests.AsQueryable();
            throw new System.NotImplementedException();
        }

        public Task<List<CustomerRequest>> GetAllCustomerRequests(CustomerRequestFilterParams customerRequestFilterParams)
        {
            var query = _db.CustomerRequests.AsQueryable();
            throw new System.NotImplementedException();
        }
    }
}