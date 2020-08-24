namespace Api.Core.Models.Api
{
    public interface IApiResponseSuccess<T> : IApiResponse<T>
    {
        T Data { get; set; }
    }
}