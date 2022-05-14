using System;
using System.Collections.Generic;
using System.Text;

namespace QuickService.LoanRepayment.Core.Entities
{
    public class CustomerRequest : BaseEntity
    {
        public CustomerRequest()
        {
            LoanRepaymentDocuments = new List<LoanRepaymentDocument>();
        }

        public string TranId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string CustomerAuthType { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TreatedBy { get; set; }
        public DateTime? TreatedDate { get; set; }
        public string RequestType { get; set; }
        public string TreatedByUnit { get; set; }
        public string RejectionReason { get; set; }
        public string Remarks { get; set; }
        public LoanRepaymentDetails LoanRepaymentDetails { get; set; }
        public ICollection<LoanRepaymentDocument> LoanRepaymentDocuments { get; set; }
    }
}