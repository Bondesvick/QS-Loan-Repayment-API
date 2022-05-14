using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuickService.LoanRepayment.Core.Entities;
using QuickService.LoanRepayment.Core.Interfaces;
using QuickService.LoanRepayment.Infrastructure.Data;
using QuickService.LoanRepayment.Infrastructure.Data.Repositories;

//using QuickService.LoanRepayment.Infrastructure.Data.Services.RedboxServiceProxies;
using QuickService.LoanRepayment.Infrastructure.Helpers;
using QuickService.LoanRepayment.Infrastructure.Helpers.Interfaces;
using QuickService.LoanRepayment.Infrastructure.OS;
using QuickService.LoanRepayment.Infrastructure.Redbox;
using QuickService.LoanRepayment.Infrastructure.Services;
using QuickService.LoanRepayment.Infrastructure.Services.RedboxServiceProxies;
using QuickService.LoanRepayment.Infrastructure.Services.RedboxServiceProxies.Interfaces;
using QuickService.LoanRepayment.Infrastructure.Services.RequestManager;

//using SharedKernel.Extensions;
//using SharedKernel.Interfaces;

namespace QuickService.LoanRepayment.Infrastructure.Extensions
{
    public static class IServiceCollectionExtension
    {
        private static void ConfigureDBContextPool(this IServiceCollection services, string connectionString)
        {
            services.AddDbContextPool<AppDbContext>(optionBuilder =>
            {
                optionBuilder.UseSqlServer(connectionString);
            });
        }

        private static void ConfigureDBContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(optionBuilder =>
            {
                optionBuilder.UseSqlServer(connectionString);
                //optionBuilder.EnableSensitiveDataLogging();
            });
        }

        public static void ResolveInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            //services.ConfigureDBContextPool(config["Data:DbConnection:ConnectionString"]);
            services.ConfigureDBContextPool(config.GetConnectionString("QuickServiceDbConn"));
            //services.ResolveSharedKernelWrappers();
            //services.AddSingleton<ILogger, AppLoggerService>();
            services.AddScoped<IRedboxManager, RedboxManager>();
            services.AddScoped<IRequestQueries, RequestQueries>();
            services.AddScoped<IRequestCommands, RequestCommands>();

            services.AddScoped<ICustomerRequestRepository, CustomerRequestRepository>();

            services.AddScoped<IAppSettings, AppSettings>();
            services.AddScoped<IAppLogger, AppLoger>();
            services.AddScoped<ISoapRequestHelper, SoapRequestHelper>();
            services.AddScoped<IRedboxRequestManagerProxy, RedboxRequestManagerProxy>();
            services.AddScoped<IRepository<Audit, long>, BaseEfRepository<Audit, long>>();
            services.AddTransient<IAuditLogService, AuditLogService>();
            //services.AddScoped<IRepository<Audit, long>, BaseEfRepository<Audit, long>>();
        }
    }
}