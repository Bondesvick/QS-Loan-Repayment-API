﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QuickService.LoanRepayment.Infrastructure.Helpers.Interfaces;

namespace QuickService.LoanRepayment.Infrastructure.Helpers
{
    public class AppLoger : IAppLogger
    {
        private readonly ILogger<AppLoger> _logger;

        public AppLoger(ILogger<AppLoger> logger)
        {
            _logger = logger;
        }

        public void Error(string message, object data = null, Exception ex = null)
        {
            if (ex != null)
                _logger.LogError(ex, message, data);
            else
                _logger.LogError(message, data);
        }

        public async Task ErrorAsync(string message, object data = null, Exception ex = null)
        {
            await Task.Run(() => Error(message, data, ex));
        }

        public void Info(string message, object data = null)
        {
            _logger.LogInformation(message, data);
        }

        public async Task InfoAsync(string message, object data = null)
        {
            await Task.Run(() => Info(message, data));
        }

        public void Warn(string message, object data = null)
        {
            _logger.LogWarning(message, data);
        }

        public async Task WarnAsync(string message, object data = null)
        {
            await Task.Run(() => Warn(message, data));
        }
    }
}