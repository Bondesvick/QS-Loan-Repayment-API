using System;
using System.Collections.Generic;
using System.Text;

namespace QuickService.LoanRepayment.Core.Entities
{
    public class LoanRepaymentDocument: BaseEntity
    {
        public string DocTitle { get; set; } //title of document
        public string DocExtension { get; set; }
        public string DocName { get; set; } //name of document is signature 
        public string DocContent { get; set; }
        public long CustomerRequestId { get; set; }
        public CustomerRequest CustomerRequest { get; set; }
    }
}
