using AutoMapper;
using Mango.Services.ProductAPI.DbContexts.Models.Dto;
using Mango.Services.ProductAPI.DbContexts.Models.Dtos;
using Microsoft.Extensions.Logging.Abstractions;

namespace Mango.Services.ProductAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, Product>();
                config.CreateMap<Product, ProductDto>();
            }, NullLoggerFactory.Instance);
            return mappingConfig;
        }
    }
}
