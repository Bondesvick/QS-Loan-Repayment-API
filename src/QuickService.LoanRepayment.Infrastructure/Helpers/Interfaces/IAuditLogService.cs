using System.Threading.Tasks;

namespace QuickService.LoanRepayment.Infrastructure.Helpers.Interfaces
{
    public interface IAuditLogService
    {
        Task<bool> AuditLog(string accountNo, string requestType, string methodName, string actionDescription, string ipAddress, string computerName);
    }
}