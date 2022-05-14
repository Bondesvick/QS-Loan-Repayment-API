using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuickService.LoanRepayment.Core.DTO.APIRequest
{
    public class GetRequestDocumentAPIRequest
    {
        [Range(1, long.MaxValue)]
        public long RequestId { get; set; }

        [Required]
        public string DocType { get; set; }

        [Range(1, long.MaxValue)]
        public long DocId { get; set; }
    }
}
