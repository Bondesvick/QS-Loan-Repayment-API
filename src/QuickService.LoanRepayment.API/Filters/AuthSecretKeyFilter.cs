using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickService.LoanRepayment.API.Filters
{
    public class AuthSecretKeyFilter : IAsyncActionFilter
    {
        private IConfiguration _config;

        public AuthSecretKeyFilter(IConfiguration config)
        {
            _config = config;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("x-stack-eb", out StringValues passedSecret))
            {
                context.Result = new BadRequestObjectResult("Invalid request");
                await Task.FromResult(false);
                return;
            }

            if (_config["AppSettings:X-Client-Secret"] != passedSecret.ToString())
            {
                context.Result = new BadRequestObjectResult("Unknown source");
                await Task.FromResult(false);
                return;
            }

            await next();
        }
    }
}