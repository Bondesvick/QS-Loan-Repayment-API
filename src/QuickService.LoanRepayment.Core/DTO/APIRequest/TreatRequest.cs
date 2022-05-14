using QuickService.LoanRepayment.Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuickService.LoanRepayment.Core.DTO.APIRequest
{
    public class TreatRequest
    {
        [Range(1, long.MaxValue)]
        public long RequestId { get; set; }

        [Required]
        public string Status { get; set; }

        public string RejectionReason { get; set; }

        public string Remarks { get; set; }

        [Required]
        public string SapId { get; set; }

        [Required]
        public string CustomerAccountNumber { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }

        public TreatRequest()
        {
            ValidationFunc = Validate;
        }

        // Helps in mocking the Validate method during unit testing
        public TreatRequest(Func<(bool validationStatus, string statusMessage)> validationFunc)
        {
            ValidationFunc = validationFunc;
        }

        public Func<(bool validationStatus, string statusMessage)> ValidationFunc { get; private set; }

        (bool validationStatus, string statusMessage) Validate()
        {

            if (!(Status.Equals(REQUEST_STATUS.DECLINED, StringComparison.OrdinalIgnoreCase)
            || Status.Equals(REQUEST_STATUS.RESOLVED, StringComparison.OrdinalIgnoreCase)
            ))
                return (false, RESPONSE_DESCRIPTION.INVALID_REQUEST_STATUS);

            if (Status.Equals(REQUEST_STATUS.DECLINED, StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(RejectionReason))
                return (false, RESPONSE_DESCRIPTION.EMPTY_REJECTION_MESSAGE);

            return (true, default);
        }

    }
}
