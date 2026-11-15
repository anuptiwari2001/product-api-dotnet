using AutoMapper;
using Mango.Services.ProductAPI.DbContexts;
using Mango.Services.ProductAPI.DbContexts.Models.Dto;
using Mango.Services.ProductAPI.DbContexts.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ProductRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var products = await _db.Products.AsNoTracking().ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto> GetProductById(int productId)
        {
            var product = await _db.Products.AsNoTracking()
                .FirstOrDefaultAsync(product => product.ProductId == productId);

            if (product is null)
            {
                throw new KeyNotFoundException($"Product with ID {productId} was not found.");
            }

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateUpdateProduct(ProductDto productDto)
        {
            var product = productDto.ProductId == 0
                ? null
                : await _db.Products.FirstOrDefaultAsync(product => product.ProductId == productDto.ProductId);

            if (product is null)
            {
                product = _mapper.Map<Product>(productDto);
                _db.Products.Add(product);
            }
            else
            {
                _mapper.Map(productDto, product);
            }

            await _db.SaveChangesAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            var product = await _db.Products.FirstOrDefaultAsync(product => product.ProductId == productId);
            if (product is null)
            {
                return false;
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
