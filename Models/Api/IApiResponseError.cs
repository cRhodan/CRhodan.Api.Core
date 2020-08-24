using System.Collections.Generic;

namespace CRhodan.Api.Core.Models.Api
{
    public interface IApiResponseError<T> : IApiResponse<T>
    {
        IEnumerable<string> Errors { get; set; }
    }
}