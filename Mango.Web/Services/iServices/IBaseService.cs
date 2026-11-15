using Mango.Web.Models;

namespace Mango.Web.Services.iServices
{
    public interface IBaseService: IDisposable
    {
        ResponseDto ResponseModel { get; set; }
        Task<T> SendAsync<T>(ApiRequest apiRequest) where T : ResponseDto, new();

    }
}
