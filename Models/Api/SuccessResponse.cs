using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Api.Core.Models.Api
{
    public class SuccessResponse<T> : IApiResponseSuccess<T>
    {
        public SuccessResponse(string correlationId, string requestId, T data)
        {
            CorrelationId = correlationId;
            RequestId = requestId;
            Data = data;
        }

        public int HttpStatusCode { get; set; } = Microsoft.AspNetCore.Http.StatusCodes.Status200OK;

        public string CorrelationId { get; set; }
        public string RequestId { get; set; }

        public bool IsSuccess => true;

        public T Data { get; set; }
    }
}
