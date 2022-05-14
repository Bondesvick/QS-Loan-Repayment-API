using System;
using System.Collections.Generic;
using System.Text;

namespace QuickService.LoanRepayment.Core.DTO.APIRequest
{
    public class ValidateBvnBobOTP
    {
        public string Bvn { get; set; }
        public DateTime Dob { get; set; }
        public string AccountNumber { get; set; }
    }
}