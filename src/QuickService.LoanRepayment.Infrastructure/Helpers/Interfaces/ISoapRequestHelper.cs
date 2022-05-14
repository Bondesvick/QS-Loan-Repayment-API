using System.Threading.Tasks;
using QuickService.LoanRepayment.Infrastructure.Redbox;

namespace QuickService.LoanRepayment.Infrastructure.Helpers.Interfaces
{
    public interface ISoapRequestHelper
    {
        Task<BaseRedboxResponse> SoapCall(string soapRequest, string soapAction, string url, string moduleId = "", string authId = "", string contenttype = "text/xml");
    }
}