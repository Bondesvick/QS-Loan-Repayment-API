using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuickService.LoanRepayment.Core.DTO.APIResponse;
using QuickService.LoanRepayment.Core.Entities;

namespace QuickService.LoanRepayment.Core.Interfaces
{
    public interface IRequestQueries
    {
        Task<List<CustomerRequestDTO>> GetCustomerRequestsByDateAsync(int pageSize, long lastIdFetched, DateTime fromDate, DateTime toDate, string requestType, string status = default);

        Task<CustomerRequest> GetCustomerRequestByIdAndSapIdWithTrackingAsync(long id, string sapId);

        Task<List<CustomerRequestDTO>> GetCustomerRequestsByDateAndTreaterAsync(int pageSize, long lastIdFetched, DateTime fromDate, DateTime toDate, string requestType,
            string sapId, string status = default);

        Task<DocumentDTO> GetRequestDocumentAsync(long customerRequestId, long docId);

        Task<CustomerRequest> GetCustomerRequestByIdWithTrackingAsync(long id);

        Task<StatisticsDTO> GetRequestStatisticsAsync(string sapId, string requestType);
    }
}