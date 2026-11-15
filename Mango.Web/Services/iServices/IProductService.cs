using Mango.Web.Models;

namespace Mango.Web.Services.iServices
{
    public interface IProductService: IBaseService
    {
        Task<T> GetAllProductsAsync<T>(string token);
        Task<T> GetProductByIdAsync<T>(int id, string token);
        Task<T> CreateProductAsync<T>(ProductDto productDto, string token);
        Task<T> UpdateProductAsync<T>(ProductDto productDto, string token);
        Task<T> DeleteProductAsync<T>(int id, string token);
        Task<T> GetAllProductsAsync<T>(string token, string categoryName);
    }
}
