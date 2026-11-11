using Mango.Services.ProductAPI.DbContexts.Models.Dto;
using Mango.Services.ProductAPI.DbContexts.Models.Dtos;
using Mango.Services.ProductAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductAPIController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto>> Get()
        {
            try
            {
                var productDtos = await _productRepository.GetProducts();
                return Ok(new ResponseDto { Result = productDtos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto
                {
                    IsSuccess = false,
                    Result = Array.Empty<ProductDto>(),
                    ErrorMessages = new List<string> { ex.ToString() }
                });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResponseDto>> GetById(int id)
        {
            try
            {
                var productDto = await _productRepository.GetProductById(id);
                return Ok(new ResponseDto { Result = productDto });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseDto
                {
                    IsSuccess = false,
                    Result = Array.Empty<ProductDto>(),
                    ErrorMessages = new List<string> { ex.Message }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto
                {
                    IsSuccess = false,
                    Result = Array.Empty<ProductDto>(),
                    ErrorMessages = new List<string> { ex.ToString() }
                });
            }
        }
    }
}
