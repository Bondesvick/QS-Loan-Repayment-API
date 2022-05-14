using System;
using System.Collections.Generic;
using System.Text;

namespace QuickService.LoanRepayment.Core.DTO.APIResponse
{
    public class LoanRepaymentDetailDTO
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AvailableBalance { get; set; }
        public string LoanAmountValue { get; set; }
        public string OutstandingBalance { get; set; }
        public string AccountSchemeType { get; set; }
        public string AccountSchemeCode { get; set; }
        public string AccountType { get; set; }
    }
}