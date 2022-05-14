using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuickService.LoanRepayment.API.Filters;

//using SharedKernel.Interfaces;
//using SharedKernel.Interfaces.Providers;

namespace QuickService.LoanRepayment.API.Controllers
{
    [ServiceFilter(typeof(AuthSecretKeyFilter), Order = 1)]
    public class BaseController : ControllerBase
    {
        //protected readonly IFileLogger _fileLogger;
        //private readonly ILogger<BaseController> _logger;

        //protected readonly IDatetimeProvider _datetimeProvider;

        //protected BaseController(IFileLogger fileLogger = default, ILogger<BaseController> logger)
        //{
        //    //_fileLogger = fileLogger;
        //    _logger = logger;
        //}
        protected BaseController()
        {
            //_fileLogger = fileLogger;
            // _logger = logger;
        }
    }
}