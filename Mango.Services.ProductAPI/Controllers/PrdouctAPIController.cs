using Mango.Services.ProductAPI.DbContexts.Models.Dto;
using Mango.Services.ProductAPI.DbContexts.Models.Dtos;
using Mango.Services.ProductAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    [Authorize(Policy = "ApiScope")]
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
        [HttpPost]
        public async Task<ActionResult<ResponseDto>> Post([FromBody] ProductDto productDto)
        {
            try
            {
                var createdProduct = await _productRepository.CreateUpdateProduct(productDto);
                return CreatedAtAction(nameof(GetById), new { id = createdProduct.ProductId }, new ResponseDto { Result = createdProduct });
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

        [HttpPut]
        public async Task<ActionResult<ResponseDto>> Put([FromBody] ProductDto productDto)
        {
            try
            {
                var updatedProduct = await _productRepository.CreateUpdateProduct(productDto);
                return Ok(new ResponseDto { Result = updatedProduct });
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

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            try
            {
                var isDeleted = await _productRepository.DeleteProduct(id);
                if (isDeleted)
                {
                    return Ok(new ResponseDto { Result = $"Product with ID {id} deleted successfully." });
                }
                else
                {
                    return NotFound(new ResponseDto
                    {
                        IsSuccess = false,
                        Result = Array.Empty<ProductDto>(),
                        ErrorMessages = new List<string> { $"Product with ID {id} not found." }
                    });
                }
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
