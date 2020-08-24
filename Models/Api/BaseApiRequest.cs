using System.ComponentModel.DataAnnotations;

namespace Api.Core.Models.Api
{
    public class BaseApiRequest : IApiRequest
    {
        [Required]
        public string CorrelationId { get; set; }
        [Required]
        public string RequestId { get; set; }
    }
}