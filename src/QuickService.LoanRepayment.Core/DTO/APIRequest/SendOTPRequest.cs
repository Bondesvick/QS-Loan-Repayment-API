using System.ComponentModel.DataAnnotations;

namespace QuickService.LoanRepayment.Core.DTO.APIRequest
{
    public class SendOTPRequest
    {
        [Required]
        public string AccountNumber { get; set; }
    }
}
