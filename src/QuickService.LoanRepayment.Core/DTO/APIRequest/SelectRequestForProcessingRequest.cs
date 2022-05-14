using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuickService.LoanRepayment.Core.DTO.APIRequest
{
    public class SelectRequestForProcessingRequest
    {
        [Range(1, long.MaxValue)]
        public long RequestId { get; set; }

        [Required]
        public string SapId { get; set; }

        [Required]
        public string Unit { get; set; }
    }
}
