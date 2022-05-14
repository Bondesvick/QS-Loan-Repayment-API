using QuickService.LoanRepayment.Core.DTO.APIRequest;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickService.LoanRepayment.Core.Entities
{
    public class LoanRepaymentDetails : BaseEntity
    {
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string SignatureContent { get; set; }
        public string SignatureExt { get; set; }
        public string RepaymentPlan { get; set; }
        public string AccountSegment { get; set; }
        public string RepaymentAmount { get; set; }
        public string LoanAccountNo { get; set; }
        public string LoanCurrentBalance { get; set; }
        public string RepaymentAcctNo { get; set; }
        public string Amount { get; set; }
        public string HaveDebitCard { get; set; }

        //public List<LoanRepaymentModel> LoanRepaymentRequestDetails { get; set; }
        public CustomerRequest CustomerRequest { get; set; }

        public long CustomerRequestId { get; set; }
    }
}