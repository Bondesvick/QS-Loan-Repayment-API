using System.Threading.Tasks;
using QuickService.LoanRepayment.Core.Entities;

namespace QuickService.LoanRepayment.Core.Interfaces
{
    public interface IRequestCommands
    {
        Task<bool> LogLoanRepaymentRequestAsync(CustomerRequest request);

        Task<bool> UpdateCustomerRequestAsync(CustomerRequest customerRequest);
    }
}