using System.Threading.Tasks;
using QuickService.LoanRepayment.Infrastructure.Redbox;

namespace QuickService.LoanRepayment.Infrastructure.Services.RedboxServiceProxies.Interfaces
{
    public interface IRedboxRequestManagerProxy
    {
        Task<BaseRequestManagerResponse<T2>> Post<T2>(string xmlReq, string module = "1", string authId = "1") where T2 : class;
    }
}