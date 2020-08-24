using System.Collections.Generic;

namespace CRhodan.Api.Core.Models.Api
{
    public class ErrorResponse<T> : IApiResponseError<T>
    {
        public ErrorResponse(string correlationId, string requestId, IEnumerable<string> errors, int? statusCode = null)
        {
            CorrelationId = correlationId;
            RequestId = requestId;
            HttpStatusCode = statusCode ?? Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError;
            Errors = errors;
        }

        public int HttpStatusCode { get; set; }
        public string CorrelationId { get; set; }
        public string RequestId { get; set; }
        public bool IsSuccess => false;
        public IEnumerable<string> Errors { get; set; }
    }
}
