using System;
using CRhodan.Api.Core.Models.Api;
using Serilog;

namespace CRhodan.Api.Core.Extensions
{
    public static class LoggingExtensions
    {
        public static void LogApiException(this IApiRequest request, Exception ex, string additionalDetails = "No additional details.")
        {
            Log.Error(ex, $"Error occurred processing request with CorrelationId {request?.CorrelationId} and Transaction Id {request?.RequestId}. Additional Details: {additionalDetails}");
        }
    }
}