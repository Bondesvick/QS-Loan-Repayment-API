using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuickService.LoanRepayment.API.Extensions;
using QuickService.LoanRepayment.Infrastructure.Data;
using QuickService.LoanRepayment.Infrastructure.Extensions;

namespace QuickService.LoanRepayment.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
            services.AddControllers().AddNewtonsoftJson(options => options.UseMemberCasing());
            services.AddSingleton(Configuration);
            services.AddHttpContextAccessor();
            services.ResolveAPIFilters();
            services.ResolveAPICors(Configuration);
            services.ResolveSwagger(Configuration);
            services.ResolveCoreServices();
            services.ResolveInfrastructureServices(Configuration);
        }

        public void Configure(IApplicationBuilder appBuilder, IWebHostEnvironment env)
        {
            appBuilder.ConfigureSwagger(Configuration);
            appBuilder.ConfigureCors(Configuration);

            if (env.IsDevelopment())
                appBuilder.UseDeveloperExceptionPage();
            else
                appBuilder.UseHsts();

            appBuilder.UseHttpsRedirection();
            appBuilder.UseRouting();
            appBuilder.UseAuthorization();
            appBuilder.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            ServiceResolver.Register(appBuilder.ApplicationServices);
        }
    }
}