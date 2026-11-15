using Microsoft.AspNetCore.Mvc;
using static Mango.Web.SD;

namespace Mango.Web.Models
{
    public class ApiRequest
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public required string Url { get; set; }
        public object? Data { get; set; }
        public required string AccessToken { get; set; }
    }
}
