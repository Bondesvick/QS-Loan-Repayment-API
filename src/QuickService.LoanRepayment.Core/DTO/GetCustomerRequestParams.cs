using System.ComponentModel.DataAnnotations;

namespace QuickService.LoanRepayment.Core.DTO
{
    public class GetCustomerRequestParams
    {
        [Required] public string Module { get; set; }
        [Required] public string TicketId { get; set; }
    }
}