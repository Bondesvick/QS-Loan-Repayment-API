using System.ComponentModel.DataAnnotations;
 
namespace QuickService.LoanRepayment.Core.DTO.APIRequest
{
    public class ValidateOTP
    {
        [Required]
        public string OtpRefence { get; set; }
        [Required]
        public string Otp { get; set; }
        [Required]
        public string AccountNumber { get; set; }
    }
}
