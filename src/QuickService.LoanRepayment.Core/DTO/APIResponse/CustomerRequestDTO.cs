using QuickService.LoanRepayment.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickService.LoanRepayment.Core.DTO.APIResponse
{
    public class CustomerRequestDTO
    {
        public CustomerRequest Request { get; set; }
        public LoanRepaymentDetailsDTO RequestDetails { get; set; }
        public List<DocumentDTO> Documents { get; set; }
    }
    public class LoanRepaymentDetailsDTO
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

    }

    public class DocumentDTO
    {
        public string DocExtension { get; set; }
        public string DocName { get; set; }
        public string DocContent { get; set; }
        public long DocId { get; set; }
    }
}
