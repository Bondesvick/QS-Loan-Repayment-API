using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuickService.LoanRepayment.Core.DTO.APIRequest
{
    public class GetDatedCustomerRequest
    {
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [Range(0, long.MaxValue)]
        public long LastIdFetched { get; set; }

        [Required]
        public string Status { get; set; }
        public string Treater { get; set; }

        [Required]
        public string RequestType { get; set; }

    }
}
