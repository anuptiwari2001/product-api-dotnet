using System.ComponentModel.DataAnnotations;

namespace Mango.Services.ProductAPI.DbContexts.Models.Dtos
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public required string Description { get; set; }
        public required string CategoryName { get; set; }

        public double Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
