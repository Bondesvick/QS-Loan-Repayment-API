using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

//using SharedKernel.Interfaces;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using QuickService.LoanRepayment.API.Filters;
using QuickService.LoanRepayment.Core.Interfaces;
using QuickService.LoanRepayment.Infrastructure.Services;

namespace QuickService.LoanRepayment.API.Extensions
{
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// Add scoped Action Filters to Service collections
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        public static void ResolveAPIFilters(this IServiceCollection services)
        {
            services.AddScoped<ModelStateValidationFilter>();
            services.AddScoped<AuthSecretKeyFilter>();
            //services.AddScoped<AuthSecretKeyFilter>();
        }

        public static void ResolveCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IRequestService, RequestService>();
            //services.AddScoped<IHttpContextUtil, SharedKernel.APIComponents.HttpContextUtils>();
        }

        public static void ResolveAPICors(this IServiceCollection services, IConfiguration config)
        {
            services.AddCors(options => ConfigureCorsPolicy(options));

            CorsOptions ConfigureCorsPolicy(CorsOptions corsOptions)
            {
                string allowedHosts = config["AppSettings:AllowedHosts"];

                if (string.IsNullOrEmpty(allowedHosts))
                    corsOptions.AddPolicy("DenyAllHost",
                                      corsPolicyBuilder => corsPolicyBuilder
                                      .AllowAnyHeader()
                                      .WithMethods(new string[4] { "POST", "PATCH", "HEAD", "OPTIONS" })
                                     );

                allowedHosts = allowedHosts.Trim();

                if (allowedHosts == "*")
                    corsOptions.AddPolicy("AllowAll",
                                     corsPolicyBuilder => corsPolicyBuilder
                                     .AllowAnyHeader()
                                     .AllowAnyMethod()
                                     .AllowAnyOrigin()
                                    );
                else
                {
                    string[] allowedHostArray;

                    if (!allowedHosts.Contains(","))
                        allowedHostArray = new string[1] { allowedHosts };
                    else
                        allowedHostArray = allowedHosts.Split(",", StringSplitOptions.RemoveEmptyEntries);

                    corsOptions.AddPolicy("AllowSpecificHost",
                                      corsPolicyBuilder => corsPolicyBuilder
                                      .AllowAnyHeader()
                                      .WithOrigins(allowedHostArray)
                                      .WithMethods(new string[4] { "POST", "PATCH", "HEAD", "OPTIONS" })
                                     );
                }

                return corsOptions;
            }
        }

        public static void ResolveSwagger(this IServiceCollection services, IConfiguration config)
        {
            bool.TryParse(config["AppSettings:EnableSwagger"], out bool enableSwagger);

            if (enableSwagger)
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Swagger StanbicIBTC Quick Service Loan Repayment", Version = "v1" });
                    c.OperationFilter<SwaggerCustomHeaderFilter>();
                    c.ResolveConflictingActions((description) => description.First());
                });
        }
    }
}