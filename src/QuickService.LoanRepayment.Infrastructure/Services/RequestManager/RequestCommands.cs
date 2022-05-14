using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QuickService.LoanRepayment.Core.Entities;
using QuickService.LoanRepayment.Core.Interfaces;
using QuickService.LoanRepayment.Infrastructure.Data; //using SharedKernel.Interfaces;

//using SharedKernel.Utils;

namespace QuickService.LoanRepayment.Infrastructure.Services.RequestManager
{
    public class RequestCommands : IRequestCommands
    {
        private readonly AppDbContext _appDbContext;

        private readonly ILogger<RequestCommands> _logger;
        //readonly IFileLogger _fileLogger;

        public RequestCommands(AppDbContext appDbContext, ILogger<RequestCommands> logger)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException("appDbContext", "Null DBContext injection");
            _logger = logger;
            //_fileLogger = fileLogger ?? throw new ArgumentNullException("fileLogger", "Null FileLogger injection");
        }

        public async Task<bool> LogLoanRepaymentRequestAsync(CustomerRequest request)
        {
            //Guard.IsNull(request, "customerRequest cannot be null.");
            //Guard.IsNull(request.LoanRepaymentDetails, "LoanRepaymentDetails cannot be null.");
            //Guard.IsNotMoreThanZero(request.LoanRepaymentDocuments.Count, "LoanRepaymentDocument cannot be empty.");

            try
            {
                _appDbContext.CustomerRequests.Add(request);

                await _appDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception: " + ex.Message, "LogLoanRepaymentRequestAsync");
                return false;
            }
        }

        public async Task<bool> UpdateCustomerRequestAsync(CustomerRequest customerRequest)
        {
            //Guard.IsNull(customerRequest, "customerRequest cannot be null.");

            _appDbContext.CustomerRequests.Update(customerRequest);

            try
            {
                await _appDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception: " + ex.Message, "UpdateCustomerRequestAsync");
                return false;
            }
        }
    }
}