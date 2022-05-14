using System;
using System.Collections.Generic;
using System.Text;

namespace QuickService.LoanRepayment.Core.DTO.APIResponse
{
    public class ValidateBvnDobResponseDto
    {
        public string Bvn { get; set; }
        public DateTime Dob { get; set; }
    }
}