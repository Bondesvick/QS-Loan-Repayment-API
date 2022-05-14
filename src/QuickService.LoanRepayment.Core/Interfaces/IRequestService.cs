using System.Collections.Generic;
using System.Threading.Tasks;
using QuickService.LoanRepayment.Core.DTO.APIRequest;
using QuickService.LoanRepayment.Core.DTO.APIResponse;

namespace QuickService.LoanRepayment.Core.Interfaces
{
    public interface IRequestService
    {
        Task<GenericAPIResponse<ValidateCustomerResponseDTO>> ValidateCustomerAndReturnCustomerDetailsAsync(ValidateCustomerRequest request);

        Task<List<CustomerRequestDTO>> GetCustomerRequestLoggedByDateAsync(GetDatedCustomerRequest request);

        Task<List<CustomerRequestDTO>> GetCustomerRequestLoggedByDateAndTreaterAsync(GetDatedCustomerRequest request);

        Task<DocumentDTO> GetRequestDocumentAsync(long customerRequestId, long docId);

        Task<StatisticsDTO> GetRequestStatisticsAsync(string sapId, string requestType);

        Task<(bool status, string statusMessage)> AssignRequestForProcessingAsync(long requestId, string sapId, string unit);

        Task<(bool status, string statusMessage)> TreatCustomerRequestAsync(TreatRequest request);

        Task NotifyCustomerAsync(string accountNumber, string message, string emailSubject, string senderEmail,
            bool isRejection = false);

        Task<(string, string)> SubmitRequestAsync(LoanRepaymentRequest payload);
    }
}