using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuickService.LoanRepayment.Core.DTO.APIRequest
{
    public class ValidateCustomerRequest
    {
        [Required]
        public string AccountNo { get; set; }

        [Required]
        public string PhoneNo { get; set; }
    }
}
