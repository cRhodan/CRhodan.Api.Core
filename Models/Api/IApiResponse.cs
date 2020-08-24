using System.Collections.Generic;
using Newtonsoft.Json;

namespace Api.Core.Models.Api
{
    public interface IApiResponse<T>
    {
        [JsonIgnore]
        int HttpStatusCode { get; set; }

        string CorrelationId { get; set; }
        string RequestId { get; set; }

        bool IsSuccess { get; }

    }
}
