namespace CRhodan.Api.Core.Models.Api
{
    public interface IApiRequest
    {
        string CorrelationId { get; set; }
        string RequestId { get; set; }
    }
}
