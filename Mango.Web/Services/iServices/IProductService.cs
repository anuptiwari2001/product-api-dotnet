using Mango.Web.Models;

namespace Mango.Web.Services.iServices
{
    public interface IProductService: IBaseService
    {
        Task<T> GetAllProductsAsync<T>(string token) where T : ResponseDto, new();
        Task<T> GetProductByIdAsync<T>(int id, string token) where T : ResponseDto, new();
        Task<T> CreateProductAsync<T>(ProductDto productDto, string token) where T : ResponseDto, new();
        Task<T> UpdateProductAsync<T>(ProductDto productDto, string token) where T : ResponseDto, new();
        Task<T> DeleteProductAsync<T>(int id, string token) where T : ResponseDto, new();
        Task<T> GetAllProductsAsync<T>(string token, string categoryName) where T : ResponseDto, new();
    }
}
