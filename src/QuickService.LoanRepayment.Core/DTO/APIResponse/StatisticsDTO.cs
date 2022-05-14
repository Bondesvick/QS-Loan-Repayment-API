using System;
using System.Collections.Generic;
using System.Text;

namespace QuickService.LoanRepayment.Core.DTO.APIResponse
{
    public class StatisticsDTO
    {
        public int AssigneToSapId { get; set; } //Request assigned to  the sap ID given
        public int AssignedToOthers { get; set; } // Request assigned to others
        public int Pending { get; set; }   //Request with Pending status 

    }
}
