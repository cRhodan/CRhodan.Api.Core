using System;
using Api.Core.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Api.Core.Extensions
{
    public static class ServiceExtensions
    {
        private const string CorsPolicyName = "BaseCorsPolicy";
        private static readonly string[] AllowedDomains = { "https://locahost:3000" };

        public static void AddDefaultServiceConfiguration(this IServiceCollection services)
        {
            services.AddHealthChecks();

            services.AddCors(o =>
            {
                o.AddPolicy(CorsPolicyName,
                    options =>
                        options.SetIsOriginAllowedToAllowWildcardSubdomains()
                            .WithOrigins(AllowedDomains)
                            .AllowAnyMethod()
                            .AllowAnyHeader())
                    ;
            });
        }

        public static void UseDefaultAppConfiguration(this IApplicationBuilder app)
        {
            app.UseCors(CorsPolicyName);

            void RequestResponseHandler(RequestResponseLoggingMiddleware.RequestProfilerModel requestProfilerModel)
            {
                Log.Information(requestProfilerModel.Request);
                Log.Information(Environment.NewLine);
                Log.Information(requestProfilerModel.Response);
            }

            app.UseMiddleware<RequestResponseLoggingMiddleware>((Action<RequestResponseLoggingMiddleware.RequestProfilerModel>)RequestResponseHandler);
        }
    }
}