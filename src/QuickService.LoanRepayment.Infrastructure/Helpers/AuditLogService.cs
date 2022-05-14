using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using QuickService.LoanRepayment.Core.Entities;
using QuickService.LoanRepayment.Infrastructure.Data.Repositories;
using QuickService.LoanRepayment.Infrastructure.Helpers.Interfaces;

namespace QuickService.LoanRepayment.Infrastructure.Helpers
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IRepository<Audit, long> _auditLogRepository;
        private readonly IAppLogger _logger;
        private readonly IConfiguration _config;

        public AuditLogService(IRepository<Audit, long> auditLogRepo, IConfiguration configuration, IAppLogger logger)
        {
            _auditLogRepository = auditLogRepo;
            _config = configuration;
            _logger = logger;
        }

        public async Task<bool> AuditLog(string accountNo, string requestType, string methodName, string actionDescription, string ipAddress, string computerName)
        {
            try
            {
                Audit audit = new Audit();
                audit.ActionBy = accountNo;
                audit.ActionDescription = actionDescription;
                audit.AuditDateTime = DateTime.Now;
                audit.ComputerName = computerName;
                audit.Hash = Cryptography.EncryptPhrase(accountNo + requestType + methodName);
                audit.IPAddress = ipAddress;
                audit.Id = Guid.NewGuid();
                audit.Method = methodName;
                audit.RequestType = requestType;
                await _auditLogRepository.AddItem(audit);
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, "AuditLog");
            }
            return await Task.FromResult(false);
        }
    }
}