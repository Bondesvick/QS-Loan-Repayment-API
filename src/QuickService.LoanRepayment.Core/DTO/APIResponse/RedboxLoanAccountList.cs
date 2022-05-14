using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace QuickService.LoanRepayment.Core.DTO.APIResponse
{
    [XmlRoot("accounts")]
    public class RedboxLoanAccountList
    {
        [XmlElement("account")]
        public List<RedboxLoanAccountModel> LoanAccts { get; set; }

    }
    [XmlType("account")]
    public class RedboxLoanAccountModel
    {
        public string accountNumber { get; set; }
        public string accountName { get; set; }
        public string accountCategory { get; set; }

    }
    public class LoanAccountResponseDTO
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountCategory { get; set; }

    }
}
