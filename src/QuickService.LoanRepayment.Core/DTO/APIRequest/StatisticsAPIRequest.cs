using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuickService.LoanRepayment.Core.DTO.APIRequest
{
    public class StatisticsAPIRequest
    {
        [Required]
        public string RequestType { get; set; }
        [Required]
        public string SapId { get; set; }
    }
}
