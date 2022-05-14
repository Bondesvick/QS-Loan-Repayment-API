using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuickService.LoanRepayment.Core.DTO.APIRequest
{
    public class LoanRepaymentRequest
    {
        [Required]
        public string AccountNumber { get; set; }

        [Required]
        public string AccountName { get; set; }

        //[Required]
        public string HaveDebitCard { get; set; }

        [Required]
        public string RepaymentPlan { get; set; }

        public string AccountSegment { get; set; }
        public string SignatureBase64 { get; set; }
        public string SignatureExt { get; set; }

        [Required]
        public string Amount { get; set; }

        public string RepaymentAmount { get; set; }
        public string LoanAccountNo { get; set; }
        public string LoanCurrentBalance { get; set; }
        public string RepaymentAcctNo { get; set; }
    }
}