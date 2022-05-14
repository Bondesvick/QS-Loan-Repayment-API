using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuickService.LoanRepayment.Core.DTO.APIRequest;
using QuickService.LoanRepayment.Core.DTO.APIResponse;
using QuickService.LoanRepayment.Core.DTO.Services;

namespace QuickService.LoanRepayment.Core.Interfaces
{
    public interface IRedboxManager
    {
        Task<ValidateCustomerResponseDTO> GetCustomerInfoByAccountNumberAsync(string accountNumber);

        Task<(bool status, string otpReference)> SendOtpAsync(string accountNumber);

        Task<bool> ValidateOtpAsync(ValidateOTP payload);

        Task<List<LoanAccountResponseDTO>> FetchLoanAccounts(string cifId);

        Task<LoanRepaymentDetailDTO> LoanAccountEnquiry(string acctNumber);

        Task SendEmailAsync(string fromEmail, string toEmail, string subject, string emailBody, string ccEmail = default);

        Task SendSMSAsync(string accountNum, string message, string recipientNum);

        Task<(bool, string)> DoStraightThroughSave(LoanRepaymentRequest payload);

        Task<CustomerInfoDTO> GetCustomerContactInfoByAccountNumberAsync(string accountNumber);

        Task<(string responseCode, string responseDescription, string segment)> GetAccountSegment(string accountNumber);

        Task<(bool status, string responseDescription)> ValidateBvnDob(string accountNumber, string bvn, DateTime dob);
    }
}