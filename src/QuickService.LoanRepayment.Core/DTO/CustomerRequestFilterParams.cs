using System;

namespace QuickServiceAdmin.Core.Model
{
    public class CustomerRequestFilterParams
    {
        public string Status { get; set; }
        public string TicketId { get; set; }

        public string Module { get; set; }
        public string Bvn { get; set; }

        public string AccountNumber { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }
}