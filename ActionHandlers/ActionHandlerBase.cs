using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRhodan.Api.Core.Extensions;
using CRhodan.Api.Core.Models.Api;

namespace CRhodan.Api.Core.ActionHandlers
{
    public abstract class ActionHandlerBase<TRequest, TResponseData> where TRequest : IApiRequest
    {
        protected abstract Task<IApiResponse<TResponseData>> ProcessAsync(TRequest request);

        public async Task<IApiResponse<TResponseData>> Execute(TRequest request)
        {
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));
                request.RequestId = request.RequestId ?? throw new ArgumentNullException(nameof(request.RequestId));

                return await ProcessAsync(request);
            }
            catch (Exception ex)
            {
                request.LogApiException(ex);
                return new ErrorResponse<TResponseData>(request?.CorrelationId, request?.RequestId, new List<string>{ex.Message});
            }
        }
    }
}
