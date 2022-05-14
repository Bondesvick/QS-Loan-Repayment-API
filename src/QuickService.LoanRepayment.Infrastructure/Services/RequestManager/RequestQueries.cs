using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuickService.LoanRepayment.Core.DTO.APIResponse;
using QuickService.LoanRepayment.Core.Entities;
using QuickService.LoanRepayment.Core.Interfaces;
using QuickService.LoanRepayment.Infrastructure.Data; //using SharedKernel.Extensions;

//using SharedKernel.Interfaces;

namespace QuickService.LoanRepayment.Infrastructure.Services.RequestManager
{
    public class RequestQueries : IRequestQueries
    {
        private readonly AppDbContext _appDbContext;

        private readonly ILogger<RequestQueries> _logger;

        //readonly IFileLogger _fileLogger;
        private readonly IConfiguration _config;

        public RequestQueries(AppDbContext appDbContext, ILogger<RequestQueries> logger, IConfiguration config)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException("appDbContext", "Null DBContext injection");
            _logger = logger;
            //_fileLogger = fileLogger ?? throw new ArgumentNullException("fileLogger", "Null FileLogger injection");
            _config = config ?? throw new ArgumentNullException("config", "Null IConfiguration injection");
        }

        public string Connectionstring
        {
            get
            {
                //return _config["Data:DbConnection:ConnectionString"];
                return _config.GetConnectionString("QuickServiceDbConn");
            }
        }

        public async Task<CustomerRequest> GetCustomerRequestByIdAndSapIdWithTrackingAsync(long id, string sapId)
        {
            try
            {
                sapId = sapId.Trim().ToLower();
                return await _appDbContext.CustomerRequests
                     .FirstOrDefaultAsync(x => x.Id == id && x.TreatedBy.ToLower() == sapId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception: " + ex.Message, "GetCustomerRequestByIdWithTrackingAsync");
            }

            return default;
        }

        public async Task<CustomerRequest> GetCustomerRequestByIdWithTrackingAsync(long id)
        {
            try
            {
                return await _appDbContext.CustomerRequests
                     .FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception: " + ex.Message, "GetCustomerRequestByIdWithTrackingAsync");
            }

            return default;
        }

        public async Task<List<CustomerRequestDTO>> GetCustomerRequestsByDateAndTreaterAsync(int pageSize, long lastIdFetched, DateTime fromDate, DateTime toDate, string requestType, string sapId, string status = null)
        {
            try
            {
                sapId = sapId.Trim().ToLower();
                requestType = requestType.Trim().ToUpper();

                IQueryable<CustomerRequest> q = _appDbContext.CustomerRequests.AsNoTracking()
                    .Where(x => x.TreatedBy.ToLower() == sapId &&
                    x.RequestType.ToUpper() == requestType
                    && EF.Functions.DateDiffDay(fromDate, x.CreatedDate) >= 0 && EF.Functions.DateDiffDay(x.CreatedDate, toDate) >= 0);

                if (!string.IsNullOrEmpty(status))
                {
                    q = q.Where(x => x.Status.ToUpper() == status);
                }

                if (lastIdFetched > 0)
                {
                    q = q.Where(x => x.Id > lastIdFetched);
                }

                return await q.OrderBy(x => x.Id)
                    .Select(x => new CustomerRequestDTO()
                    {
                        Request = x,
                        RequestDetails = new LoanRepaymentDetailsDTO()
                        {
                            AccountNumber = x.LoanRepaymentDetails.AccountNumber,
                            AccountName = x.LoanRepaymentDetails.AccountName,
                            AccountSegment = x.LoanRepaymentDetails.AccountSegment,
                            LoanCurrentBalance = x.LoanRepaymentDetails.LoanCurrentBalance,
                            LoanAccountNo = x.LoanRepaymentDetails.LoanAccountNo,
                            RepaymentAmount = x.LoanRepaymentDetails.RepaymentAmount,
                            Amount = x.LoanRepaymentDetails.Amount,
                            RepaymentAcctNo = x.LoanRepaymentDetails.RepaymentAcctNo,
                            RepaymentPlan = x.LoanRepaymentDetails.RepaymentPlan
                        },
                        Documents = x.LoanRepaymentDocuments.Select(t => new DocumentDTO()
                        {
                            DocId = t.Id,
                            DocName = t.DocName,
                            DocContent = t.DocContent,
                            DocExtension = t.DocExtension
                        }).ToList()
                    })
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception: " + ex.Message, "GetCustomerRequestsByDateAndTreaterAsync");
            }

            return default;
        }

        public async Task<List<CustomerRequestDTO>> GetCustomerRequestsByDateAsync(int pageSize, long lastIdFetched, DateTime fromDate, DateTime toDate, string requestType, string status = null)
        {
            try
            {
                requestType = requestType.Trim().ToUpper();

                IQueryable<CustomerRequest> q = _appDbContext.CustomerRequests.AsNoTracking()
                    .Where(x =>
                    x.RequestType.ToUpper() == requestType
                    && EF.Functions.DateDiffDay(fromDate, x.CreatedDate) >= 0 && EF.Functions.DateDiffDay(x.CreatedDate, toDate) >= 0);

                if (!string.IsNullOrEmpty(status))
                {
                    status = status.ToLower();
                    q = q.Where(x => x.Status.ToLower() == status);
                }

                if (lastIdFetched > 0)
                {
                    q = q.Where(x => x.Id > lastIdFetched);
                }

                return await q.OrderBy(x => x.Id)
                    .Select(x => new CustomerRequestDTO()
                    {
                        Request = x,
                        RequestDetails = new LoanRepaymentDetailsDTO()
                        {
                            AccountNumber = x.LoanRepaymentDetails.AccountNumber,
                            AccountName = x.LoanRepaymentDetails.AccountName,
                            AccountSegment = x.LoanRepaymentDetails.AccountSegment,
                            LoanCurrentBalance = x.LoanRepaymentDetails.LoanCurrentBalance,
                            LoanAccountNo = x.LoanRepaymentDetails.LoanAccountNo,
                            RepaymentAmount = x.LoanRepaymentDetails.RepaymentAmount,
                            Amount = x.LoanRepaymentDetails.Amount,
                            RepaymentAcctNo = x.LoanRepaymentDetails.RepaymentAcctNo,
                            RepaymentPlan = x.LoanRepaymentDetails.RepaymentPlan
                        },
                        Documents = x.LoanRepaymentDocuments.Select(t => new DocumentDTO()
                        {
                            DocId = t.Id,
                            DocName = t.DocName,
                            DocContent = t.DocContent,
                            DocExtension = t.DocExtension
                        }).ToList()
                    })
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception: " + ex.Message, "GetCustomerRequestsByDateAsync");
            }

            return default;
        }

        public async Task<DocumentDTO> GetRequestDocumentAsync(long customerRequestId, long docId)
        {
            try
            {
                return await _appDbContext.LoanRepaymentDocuments.AsNoTracking()
                    .Where(x => x.CustomerRequestId == customerRequestId && x.Id == docId)
                    .Select(x => new DocumentDTO()
                    {
                        DocExtension = x.DocExtension,
                        DocContent = x.DocContent
                    }).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception: " + ex.Message, "GetRequestDocumentAsync");
            }

            return default;
        }

        public async Task<StatisticsDTO> GetRequestStatisticsAsync(string sapId, string requestType)
        {
            try
            {
                using var connection = new SqlConnection(Connectionstring);
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@SapId", sapId),
                    new SqlParameter("@RequestType", requestType)
                };

                DataSet dataSet = await GetDataSetAsync(connection, "GetCustomerRequestCounts", parameters);

                var AssigneToSapIdDataTable = dataSet?.Tables?[0];
                var AssignedToOthersDataTable = dataSet?.Tables?[1];
                var PendingDataTable = dataSet?.Tables?[2];

                return new StatisticsDTO()
                {
                    AssignedToOthers = (int)AssignedToOthersDataTable.Rows[0][0],
                    AssigneToSapId = (int)AssigneToSapIdDataTable.Rows[0][0],
                    Pending = (int)PendingDataTable.Rows[0][0]
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception: " + ex.Message, "GetCustomerRequestsByDateAsync");
            }

            return default;
        }

        private async Task<DataSet> GetDataSetAsync(SqlConnection connection, string storedProcName, params SqlParameter[] parameters)
        {
            using var command = new SqlCommand(storedProcName, connection) { CommandType = CommandType.StoredProcedure };
            command.Parameters.AddRange(parameters);

            var result = new DataSet();
            await connection.OpenAsync();
            using var dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(result);

            if (!(connection.State == ConnectionState.Closed))
            {
                await connection.CloseAsync();
            }
            return result;
        }
    }
}